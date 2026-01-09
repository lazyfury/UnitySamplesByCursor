using System;
using System.Collections.Generic;
using System.Linq;
using PokerEngine.Core.Interfaces;
using PokerEngine.Core.Types;

namespace PokerEngine.Core.Systems
{
    /// <summary>
    /// 下注系统
    /// </summary>
    public class BettingSystem
    {
        private int _currentBet;
        private int _currentRaise;
        private int _smallBlind;
        private int _bigBlind;

        public int CurrentBet => _currentBet;
        public int CurrentRaise => _currentRaise;

        public BettingSystem(int smallBlind, int bigBlind)
        {
            _smallBlind = smallBlind;
            _bigBlind = bigBlind;
            Reset();
        }

        public void Reset()
        {
            _currentBet = 0;
            _currentRaise = 0;
        }

        public void PostBlinds(List<IPokerPlayer> players, int dealerPosition)
        {
            Reset();

            int smallBlindPos = (dealerPosition + 1) % players.Count;
            int bigBlindPos = (dealerPosition + 2) % players.Count;

            if (players.Count == 2)
            {
                // 两人局：庄家是小盲，对手是大盲
                smallBlindPos = dealerPosition;
                bigBlindPos = (dealerPosition + 1) % 2;
            }

            var smallBlindPlayer = players[smallBlindPos];
            var bigBlindPlayer = players[bigBlindPos];

            int smallBlindAmount = Math.Min(_smallBlind, smallBlindPlayer.Chips);
            int bigBlindAmount = Math.Min(_bigBlind, bigBlindPlayer.Chips);

            smallBlindPlayer.Bet(smallBlindAmount);
            bigBlindPlayer.Bet(bigBlindAmount);

            _currentBet = bigBlindAmount;
            _currentRaise = bigBlindAmount - smallBlindAmount;
        }

        public bool IsValidAction(IPokerPlayer player, ActionType actionType, int amount)
        {
            if (player.HasFolded || !player.IsActive)
                return false;

            if (player.IsAllIn)
                return false;

            int chipsToCall = _currentBet - player.CurrentBet;

            switch (actionType)
            {
                case ActionType.Fold:
                    return true;

                case ActionType.Check:
                    return chipsToCall == 0;

                case ActionType.Call:
                    return chipsToCall > 0 && chipsToCall <= player.Chips;

                case ActionType.Bet:
                    if (_currentBet > 0)
                        return false; // 不能下注，只能加注
                    return amount >= _bigBlind && amount <= player.Chips;

                case ActionType.Raise:
                    if (_currentBet == 0)
                        return false; // 没有下注，不能加注
                    int minRaise = _currentBet + _currentRaise;
                    int totalNeeded = minRaise - player.CurrentBet;
                    return amount >= minRaise && totalNeeded <= player.Chips;

                case ActionType.AllIn:
                    return player.Chips > 0;

                default:
                    return false;
            }
        }

        public void ProcessAction(IPokerPlayer player, ActionType actionType, int amount)
        {
            if (!IsValidAction(player, actionType, amount))
                throw new System.ArgumentException("无效的行动");

            int chipsToCall = _currentBet - player.CurrentBet;

            switch (actionType)
            {
                case ActionType.Fold:
                    player.Fold();
                    break;

                case ActionType.Check:
                    // 不需要做任何事
                    break;

                case ActionType.Call:
                    player.Bet(chipsToCall);
                    break;

                case ActionType.Bet:
                    player.Bet(amount);
                    _currentBet = amount;
                    _currentRaise = amount;
                    break;

                case ActionType.Raise:
                    int raiseAmount = amount - player.CurrentBet;
                    player.Bet(amount);
                    _currentRaise = amount - _currentBet;
                    _currentBet = amount;
                    break;

                case ActionType.AllIn:
                    int allInAmount = player.Chips;
                    player.Bet(allInAmount);
                    if (player.CurrentBet > _currentBet)
                    {
                        _currentBet = player.CurrentBet;
                        _currentRaise = player.CurrentBet - (_currentBet - _currentRaise);
                    }
                    break;
            }
        }

        public bool IsBettingRoundComplete(List<IPokerPlayer> activePlayers)
        {
            // 检查是否所有活跃玩家都已行动
            var playersToAct = activePlayers.Where(p => !p.HasFolded && !p.IsAllIn).ToList();
            
            if (playersToAct.Count == 0)
                return true;

            // 检查是否所有玩家都已跟注或全押
            bool allActed = playersToAct.All(p => p.CurrentBet == _currentBet || p.IsAllIn);
            
            // 检查是否至少有一个玩家行动过（防止所有人都过牌的情况）
            bool someoneActed = playersToAct.Any(p => p.CurrentBet > 0 || p.HasFolded);

            return allActed && someoneActed;
        }

        public int GetChipsToCall(IPokerPlayer player)
        {
            return _currentBet - player.CurrentBet;
        }
    }
}
