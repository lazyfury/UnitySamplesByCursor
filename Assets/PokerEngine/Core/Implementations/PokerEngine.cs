using System;
using System.Collections.Generic;
using System.Linq;
using PokerEngine.Core.Interfaces;
using PokerEngine.Core.Systems;
using PokerEngine.Core.Types;

namespace PokerEngine.Core.Implementations
{
    /// <summary>
    /// 德州扑克引擎实现
    /// </summary>
    public class PokerGameEngine : IPokerEngine
    {
        private GameState _state;
        private GamePhase _phase;
        private IDeck _deck;
        private BettingSystem _bettingSystem;
        private PotManager _potManager;
        private List<IPokerPlayer> _players;
        private List<Card> _communityCards;
        private int _dealerPosition;
        private int _currentPlayerPosition;
        private Dictionary<int, HandResult> _handResults;

        // 事件
        public event Action<GamePhase> OnPhaseChanged;
        public event Action<int> OnPotChanged;
        public event Action<IPokerPlayer, PlayerAction> OnPlayerAction;
        public event Action<List<IPokerPlayer>> OnHandFinished;
        public event Action<IPokerPlayer> OnPlayerEliminated;

        // 属性
        public GameState State
        {
            get => _state;
            private set => _state = value;
        }

        public GamePhase Phase
        {
            get => _phase;
            private set
            {
                if (_phase != value)
                {
                    _phase = value;
                    OnPhaseChanged?.Invoke(_phase);
                }
            }
        }

        public int SmallBlind { get; set; }
        public int BigBlind { get; set; }
        public int CurrentPot => _potManager.TotalPot;
        public int CurrentBet => _bettingSystem.CurrentBet;
        public int DealerPosition => _dealerPosition;
        public int CurrentPlayerPosition => _currentPlayerPosition;
        public List<Card> CommunityCards => new List<Card>(_communityCards);
        public List<IPokerPlayer> Players => new List<IPokerPlayer>(_players);
        public List<IPokerPlayer> ActivePlayers => _players.Where(p => p.IsActive && !p.HasFolded && p.Chips > 0).ToList();

        public PokerGameEngine()
        {
            _deck = new Deck();
            _potManager = new PotManager();
            _players = new List<IPokerPlayer>();
            _communityCards = new List<Card>();
            _handResults = new Dictionary<int, HandResult>();
            _state = GameState.NotInitialized;
            _phase = GamePhase.NotStarted;
            SmallBlind = 10;
            BigBlind = 20;
        }

        public void Initialize(List<IPokerPlayer> players, int smallBlind, int bigBlind)
        {
            if (players == null || players.Count < 2)
                throw new ArgumentException("至少需要2名玩家");
            if (players.Count > 10)
                throw new ArgumentException("最多支持10名玩家");

            _players = new List<IPokerPlayer>(players);
            SmallBlind = smallBlind;
            BigBlind = bigBlind;
            _bettingSystem = new BettingSystem(smallBlind, bigBlind);
            _dealerPosition = 0;
            _state = GameState.Waiting;
            _phase = GamePhase.NotStarted;
        }

        public void StartNewHand()
        {
            if (_state == GameState.NotInitialized)
                throw new InvalidOperationException("引擎未初始化");

            // 移除没有筹码的玩家
            _players.RemoveAll(p => p.Chips <= 0);
            if (_players.Count < 2)
            {
                _state = GameState.Finished;
                return;
            }

            // 重置所有玩家
            foreach (var player in _players)
            {
                player.ResetForNewHand();
            }

            // 移动庄家位置
            _dealerPosition = (_dealerPosition + 1) % _players.Count;
            SetBlindPositions();

            // 重置牌组和公共牌
            _deck.Reset();
            _communityCards.Clear();
            _potManager.Reset();
            _bettingSystem.Reset();
            _handResults.Clear();

            // 发底牌
            foreach (var player in _players)
            {
                player.AddCard(_deck.DealCard());
                player.AddCard(_deck.DealCard());
            }

            // 下盲注
            _bettingSystem.PostBlinds(_players, _dealerPosition);

            // 设置当前行动玩家（大盲注的下一位）
            _currentPlayerPosition = GetNextActivePlayerPosition((_dealerPosition + 2) % _players.Count);

            _state = GameState.Playing;
            Phase = GamePhase.PreFlop;
        }

        public void ProcessPlayerAction(int playerId, ActionType actionType, int amount = 0)
        {
            if (_state != GameState.Playing)
                throw new InvalidOperationException("游戏未在进行中");

            var player = _players.FirstOrDefault(p => p.Id == playerId);
            if (player == null)
                throw new ArgumentException("玩家不存在");

            if (player.Id != _players[_currentPlayerPosition].Id)
                throw new InvalidOperationException("不是该玩家的行动时间");

            // 处理行动
            _bettingSystem.ProcessAction(player, actionType, amount);
            OnPlayerAction?.Invoke(player, new PlayerAction(playerId, actionType, amount));

            // 检查是否进入下一阶段
            if (_bettingSystem.IsBettingRoundComplete(ActivePlayers))
            {
                NextPhase();
            }
            else
            {
                // 移动到下一个玩家
                MoveToNextPlayer();
            }
        }

        public void NextPhase()
        {
            // 收集下注到底池
            _potManager.CollectBets(_players);
            OnPotChanged?.Invoke(CurrentPot);

            // 检查是否只剩一个玩家
            var activePlayers = ActivePlayers;
            if (activePlayers.Count <= 1)
            {
                EndHand();
                return;
            }

            // 进入下一阶段
            switch (Phase)
            {
                case GamePhase.PreFlop:
                    // 发翻牌（3张）
                    _communityCards.Add(_deck.DealCard());
                    _communityCards.Add(_deck.DealCard());
                    _communityCards.Add(_deck.DealCard());
                    Phase = GamePhase.Flop;
                    _bettingSystem.Reset();
                    _currentPlayerPosition = GetNextActivePlayerPosition(_dealerPosition);
                    break;

                case GamePhase.Flop:
                    // 发转牌（1张）
                    _communityCards.Add(_deck.DealCard());
                    Phase = GamePhase.Turn;
                    _bettingSystem.Reset();
                    _currentPlayerPosition = GetNextActivePlayerPosition(_dealerPosition);
                    break;

                case GamePhase.Turn:
                    // 发河牌（1张）
                    _communityCards.Add(_deck.DealCard());
                    Phase = GamePhase.River;
                    _bettingSystem.Reset();
                    _currentPlayerPosition = GetNextActivePlayerPosition(_dealerPosition);
                    break;

                case GamePhase.River:
                    // 进入摊牌
                    Phase = GamePhase.Showdown;
                    EndHand();
                    break;
            }
        }

        public void EndHand()
        {
            Phase = GamePhase.Showdown;

            // 收集剩余下注
            _potManager.CollectBets(_players);
            OnPotChanged?.Invoke(CurrentPot);

            // 评估所有未弃牌玩家的手牌
            var activePlayers = ActivePlayers;
            if (activePlayers.Count == 1)
            {
                // 只有一人，直接获胜
                var winner = activePlayers[0];
                winner.Chips += CurrentPot;
                OnHandFinished?.Invoke(new List<IPokerPlayer> { winner });
            }
            else
            {
                // 评估所有玩家的手牌
                _handResults.Clear();
                foreach (var player in activePlayers)
                {
                    var result = EvaluateHand(player.HoleCards, _communityCards);
                    _handResults[player.Id] = result;
                }

                // 找出获胜者
                var winners = GetWinners();
                _potManager.DistributePot(winners, _handResults);
                OnHandFinished?.Invoke(winners);
            }

            // 移除没有筹码的玩家
            var eliminatedPlayers = _players.Where(p => p.Chips <= 0).ToList();
            foreach (var player in eliminatedPlayers)
            {
                OnPlayerEliminated?.Invoke(player);
            }
            _players.RemoveAll(p => p.Chips <= 0);

            // 检查游戏是否结束
            if (_players.Count < 2)
            {
                _state = GameState.Finished;
            }
            else
            {
                Phase = GamePhase.Finished;
            }
        }

        public void RemovePlayer(int playerId)
        {
            var player = _players.FirstOrDefault(p => p.Id == playerId);
            if (player != null)
            {
                _players.Remove(player);
                if (_players.Count < 2 && _state == GameState.Playing)
                {
                    _state = GameState.Finished;
                }
            }
        }

        public HandResult EvaluateHand(List<Card> holeCards, List<Card> communityCards)
        {
            return HandEvaluator.EvaluateHand(holeCards, communityCards);
        }

        public List<IPokerPlayer> GetWinners()
        {
            if (_handResults.Count == 0)
                return new List<IPokerPlayer>();

            var activePlayers = ActivePlayers;
            if (activePlayers.Count == 0)
                return new List<IPokerPlayer>();

            // 找出最高分的手牌
            var bestResult = activePlayers
                .Select(p => _handResults[p.Id])
                .OrderByDescending(r => r.Score)
                .First();

            // 返回所有拥有最高分手牌的玩家
            return activePlayers
                .Where(p => HandEvaluator.CompareHands(_handResults[p.Id], bestResult) == 0)
                .ToList();
        }

        private void SetBlindPositions()
        {
            // 重置所有玩家的盲注位置
            foreach (var player in _players)
            {
                player.IsDealer = false;
                player.IsSmallBlind = false;
                player.IsBigBlind = false;
            }

            if (_players.Count == 2)
            {
                // 两人局：庄家是小盲，对手是大盲
                _players[_dealerPosition].IsDealer = true;
                _players[_dealerPosition].IsSmallBlind = true;
                _players[(_dealerPosition + 1) % 2].IsBigBlind = true;
            }
            else
            {
                // 多人局：庄家、小盲、大盲
                _players[_dealerPosition].IsDealer = true;
                _players[(_dealerPosition + 1) % _players.Count].IsSmallBlind = true;
                _players[(_dealerPosition + 2) % _players.Count].IsBigBlind = true;
            }
        }

        private int GetNextActivePlayerPosition(int startPosition)
        {
            int position = startPosition;
            int attempts = 0;

            while (attempts < _players.Count)
            {
                var player = _players[position];
                if (player.IsActive && !player.HasFolded && player.Chips > 0)
                {
                    return position;
                }
                position = (position + 1) % _players.Count;
                attempts++;
            }

            return startPosition; // 如果找不到，返回起始位置
        }

        private void MoveToNextPlayer()
        {
            _currentPlayerPosition = GetNextActivePlayerPosition((_currentPlayerPosition + 1) % _players.Count);
        }
    }
}
