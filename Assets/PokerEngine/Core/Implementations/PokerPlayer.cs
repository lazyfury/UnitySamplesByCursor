using System.Collections.Generic;
using PokerEngine.Core.Interfaces;
using PokerEngine.Core.Types;

namespace PokerEngine.Core.Implementations
{
    /// <summary>
    /// 玩家实现
    /// </summary>
    public class PokerPlayer : IPokerPlayer
    {
        public int Id { get; }
        public string Name { get; }
        public int Chips { get; set; }
        public int CurrentBet { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllIn => Chips == 0 && CurrentBet > 0;
        public bool HasFolded { get; set; }
        public List<Card> HoleCards { get; }
        public int SeatPosition { get; set; }
        public bool IsDealer { get; set; }
        public bool IsSmallBlind { get; set; }
        public bool IsBigBlind { get; set; }

        public PokerPlayer(int id, string name, int startingChips)
        {
            Id = id;
            Name = name;
            Chips = startingChips;
            CurrentBet = 0;
            IsActive = true;
            HasFolded = false;
            HoleCards = new List<Card>();
            SeatPosition = 0;
            IsDealer = false;
            IsSmallBlind = false;
            IsBigBlind = false;
        }

        public void AddCard(Card card)
        {
            if (HoleCards.Count >= 2)
                throw new System.InvalidOperationException("玩家最多只能有2张底牌");
            HoleCards.Add(card);
        }

        public void ClearCards()
        {
            HoleCards.Clear();
        }

        public void Bet(int amount)
        {
            if (amount < 0)
                throw new System.ArgumentException("下注金额不能为负");
            if (amount > Chips)
                throw new System.ArgumentException("下注金额不能超过筹码数");

            int actualBet = System.Math.Min(amount, Chips);
            Chips -= actualBet;
            CurrentBet += actualBet;
        }

        public void Fold()
        {
            HasFolded = true;
            IsActive = false;
        }

        public void ResetForNewHand()
        {
            CurrentBet = 0;
            HasFolded = false;
            IsActive = true;
            ClearCards();
            IsDealer = false;
            IsSmallBlind = false;
            IsBigBlind = false;
        }
    }
}
