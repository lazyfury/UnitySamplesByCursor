using System;
using System.Collections.Generic;
using System.Linq;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Systems;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Implementations
{
    /// <summary>
    /// 三国杀风格卡牌战斗引擎实现
    /// </summary>
    public class SGSKGameEngine : ISGSKEngine
    {
        private GameState _state;
        private GamePhase _phase;
        private int _currentPlayerId;
        private List<IPlayer> _players;
        private IDeck _deck;
        private List<Card> _discardPile;

        // 事件
        public event Action<GamePhase> OnPhaseChanged;
        public event Action<int> OnTurnStarted;
        public event Action<int> OnTurnEnded;
        public event Action<IPlayer, Card> OnCardPlayed;
        public event Action<IPlayer, Card> OnCardUsed;
        public event Action<DamageResult> OnDamageDealt;
        public event Action<int, int> OnPlayerDied;
        public event Action<IPlayer> OnPlayerStateChanged;

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

        public int CurrentPlayerId => _currentPlayerId;
        public List<IPlayer> Players => new List<IPlayer>(_players);
        public List<IPlayer> AlivePlayers => _players.Where(p => p.State == PlayerState.Alive).ToList();
        public IDeck Deck => _deck;
        public List<Card> DiscardPile => _discardPile;

        public SGSKGameEngine()
        {
            _players = new List<IPlayer>();
            _deck = new Deck();
            _discardPile = new List<Card>();
            _state = GameState.NotInitialized;
            _phase = GamePhase.Prepare;
        }

        public void Initialize(List<IPlayer> players)
        {
            if (players == null || players.Count < 2)
                throw new ArgumentException("至少需要2名玩家");
            if (players.Count > 8)
                throw new ArgumentException("最多支持8名玩家");

            _players = new List<IPlayer>(players);
            _deck.InitializeStandardDeck();
            _discardPile.Clear();
            _state = GameState.Playing;
        }

        public void StartGame()
        {
            if (_state == GameState.NotInitialized)
                throw new InvalidOperationException("引擎未初始化");

            _state = GameState.Playing;
            _currentPlayerId = _players[0].Id;
            StartTurn(_currentPlayerId);
        }

        public void EndGame()
        {
            _state = GameState.Finished;
        }

        public void StartTurn(int playerId)
        {
            var player = GetPlayer(playerId);
            if (player == null || player.State != PlayerState.Alive)
                return;

            _currentPlayerId = playerId;
            Phase = GamePhase.Prepare;
            player.HasUsedSlash = false;

            OnTurnStarted?.Invoke(playerId);

            // 自动进入下一阶段
            NextPhase();
        }

        public void EndTurn()
        {
            OnTurnEnded?.Invoke(_currentPlayerId);

            // 切换到下一个存活玩家
            var currentIndex = _players.FindIndex(p => p.Id == _currentPlayerId);
            int nextIndex = (currentIndex + 1) % _players.Count;
            
            // 找到下一个存活玩家
            for (int i = 0; i < _players.Count; i++)
            {
                if (_players[nextIndex].State == PlayerState.Alive)
                {
                    StartTurn(_players[nextIndex].Id);
                    return;
                }
                nextIndex = (nextIndex + 1) % _players.Count;
            }

            // 如果没有存活玩家，结束游戏
            EndGame();
        }

        public void NextPhase()
        {
            var player = GetPlayer(_currentPlayerId);
            if (player == null)
                return;

            switch (Phase)
            {
                case GamePhase.Prepare:
                    Phase = GamePhase.Judgment;
                    ProcessDelayedTrick(_currentPlayerId);
                    break;
                case GamePhase.Judgment:
                    Phase = GamePhase.Draw;
                    DrawCard(_currentPlayerId, 2);
                    break;
                case GamePhase.Draw:
                    Phase = GamePhase.Play;
                    // 出牌阶段由玩家主动操作
                    break;
                case GamePhase.Play:
                    Phase = GamePhase.Discard;
                    // 检查手牌数是否超过血量
                    if (player.Hand.Count > player.Hp)
                    {
                        // 需要弃牌，这里简化处理
                        // 实际应该让玩家选择弃哪些牌
                        int discardCount = player.Hand.Count - player.Hp;
                        for (int i = 0; i < discardCount && player.Hand.Count > 0; i++)
                        {
                            DiscardCard(_currentPlayerId, player.Hand[0].Id);
                        }
                    }
                    break;
                case GamePhase.Discard:
                    Phase = GamePhase.Finish;
                    break;
                case GamePhase.Finish:
                    EndTurn();
                    return;
            }
        }

        public bool PlayCard(int playerId, int cardId, int? targetId = null)
        {
            if (_state != GameState.Playing || Phase != GamePhase.Play)
                return false;

            var player = GetPlayer(playerId);
            if (player == null || player.Id != _currentPlayerId)
                return false;

            var card = player.GetCard(cardId);
            if (card == null)
                return false;

            List<IPlayer> targets = null;
            if (targetId.HasValue)
            {
                var target = GetPlayer(targetId.Value);
                if (target != null)
                    targets = new List<IPlayer> { target };
            }

            // 检查是否可以打出
            if (!CardEffectSystem.CanPlayCard(this, player, card, targets))
                return false;

            // 执行卡牌效果
            if (CardEffectSystem.ExecuteCardEffect(this, player, card, targets))
            {
                player.RemoveCard(cardId);
                OnCardPlayed?.Invoke(player, card);
                return true;
            }

            return false;
        }

        public bool UseCard(int playerId, int cardId, int? targetId = null)
        {
            var player = GetPlayer(playerId);
            if (player == null)
                return false;

            var card = player.GetCard(cardId);
            if (card == null)
                return false;

            List<IPlayer> targets = null;
            if (targetId.HasValue)
            {
                var target = GetPlayer(targetId.Value);
                if (target != null)
                    targets = new List<IPlayer> { target };
            }

            // 使用卡牌（如桃）
            if (card.Name.Contains("桃"))
            {
                if (player.Hp < player.MaxHp || player.State == PlayerState.Dying)
                {
                    RecoverHp(playerId, 1);
                    player.RemoveCard(cardId);
                    _discardPile.Add(card);
                    OnCardUsed?.Invoke(player, card);
                    return true;
                }
            }

            return false;
        }

        public void DiscardCard(int playerId, int cardId)
        {
            var player = GetPlayer(playerId);
            if (player == null)
                return;

            var card = player.GetCard(cardId);
            if (card != null)
            {
                player.RemoveCard(cardId);
                _discardPile.Add(card);
            }
        }

        public void DrawCard(int playerId, int count = 1)
        {
            var player = GetPlayer(playerId);
            if (player == null)
                return;

            // 如果牌堆空了，将弃牌堆洗回牌堆
            if (_deck.IsEmpty && _discardPile.Count > 0)
            {
                _deck.AddCards(_discardPile);
                _discardPile.Clear();
                _deck.Shuffle();
            }

            var cards = _deck.Draw(count);
            foreach (var card in cards)
            {
                if (card != null)
                    player.AddCard(card);
            }
        }

        public bool Attack(int sourceId, int targetId, int? cardId = null)
        {
            Card slashCard = null;
            if (cardId.HasValue)
            {
                var source = GetPlayer(sourceId);
                slashCard = source?.GetCard(cardId.Value);
            }

            return CombatSystem.ProcessAttack(this, sourceId, targetId, slashCard);
        }

        public bool Dodge(int playerId, int cardId)
        {
            var player = GetPlayer(playerId);
            if (player == null)
                return false;

            var card = player.GetCard(cardId);
            if (card == null || !card.Name.Contains("闪"))
                return false;

            player.RemoveCard(cardId);
            _discardPile.Add(card);
            return true;
        }

        public DamageResult DealDamage(int sourceId, int targetId, int amount, DamageType type = DamageType.Normal)
        {
            var source = GetPlayer(sourceId);
            var target = GetPlayer(targetId);

            if (source == null || target == null || target.State != PlayerState.Alive)
            {
                return new DamageResult
                {
                    SourceId = sourceId,
                    TargetId = targetId,
                    Amount = 0,
                    Type = type,
                    Dodged = true
                };
            }

            var result = new DamageResult
            {
                SourceId = sourceId,
                TargetId = targetId,
                Amount = amount,
                Type = type,
                Dodged = false
            };

            target.Hp -= amount;
            if (target.Hp <= 0)
            {
                target.Hp = 0;
                target.State = PlayerState.Dying;
                OnPlayerStateChanged?.Invoke(target);

                // 检查是否死亡
                if (target.Hp <= 0)
                {
                    target.State = PlayerState.Dead;
                    OnPlayerDied?.Invoke(sourceId, targetId);
                }
            }

            OnDamageDealt?.Invoke(result);
            return result;
        }

        public bool RecoverHp(int playerId, int amount)
        {
            var player = GetPlayer(playerId);
            if (player == null)
                return false;

            int oldHp = player.Hp;
            player.Hp = System.Math.Min(player.Hp + amount, player.MaxHp);
            
            if (player.State == PlayerState.Dying && player.Hp > 0)
            {
                player.State = PlayerState.Alive;
                OnPlayerStateChanged?.Invoke(player);
            }

            return player.Hp > oldHp;
        }

        public Card Judge(int playerId)
        {
            return JudgmentSystem.ExecuteJudgment(this, playerId);
        }

        public void ProcessDelayedTrick(int playerId)
        {
            var player = GetPlayer(playerId);
            if (player == null)
                return;

            var delayedTricks = new List<Card>(player.JudgmentArea);
            foreach (var trick in delayedTricks)
            {
                var judgmentCard = Judge(playerId);
                if (judgmentCard == null)
                    continue;

                bool effect = false;
                switch (trick.Name)
                {
                    case "乐不思蜀":
                        effect = JudgmentSystem.CheckIndulgence(judgmentCard);
                        break;
                    case "闪电":
                        effect = JudgmentSystem.CheckLightning(judgmentCard);
                        if (effect)
                        {
                            DealDamage(0, playerId, 3, DamageType.Thunder);
                        }
                        break;
                    case "兵粮寸断":
                        effect = JudgmentSystem.CheckSupplyShortage(judgmentCard);
                        break;
                }

                player.RemoveJudgmentCard(trick.Id);
                _discardPile.Add(trick);

                if (effect)
                {
                    // 效果生效
                    if (trick.Name == "乐不思蜀")
                    {
                        // 跳过出牌阶段
                        if (Phase == GamePhase.Play)
                        {
                            NextPhase();
                        }
                    }
                    else if (trick.Name == "兵粮寸断")
                    {
                        // 跳过摸牌阶段
                        if (Phase == GamePhase.Draw)
                        {
                            NextPhase();
                        }
                    }
                }
            }
        }

        public int GetDistance(int fromId, int toId)
        {
            return DistanceSystem.CalculateDistance(this, fromId, toId);
        }

        public bool IsInRange(int fromId, int toId)
        {
            return DistanceSystem.IsInAttackRange(this, fromId, toId);
        }

        public IPlayer GetPlayer(int playerId)
        {
            return _players.FirstOrDefault(p => p.Id == playerId);
        }

        public List<Card> GetPlayerHand(int playerId)
        {
            var player = GetPlayer(playerId);
            return player?.Hand ?? new List<Card>();
        }

        public List<Card> GetPlayerEquipments(int playerId)
        {
            var player = GetPlayer(playerId);
            if (player == null)
                return new List<Card>();

            return player.Equipments.Values.ToList();
        }
    }
}
