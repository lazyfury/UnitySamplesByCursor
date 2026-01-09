using System.Collections.Generic;
using PokerEngine.Core.Types;

namespace PokerEngine.Core.Interfaces
{
    /// <summary>
    /// 玩家接口
    /// </summary>
    public interface IPokerPlayer
    {
        int Id { get; }
        string Name { get; }
        int Chips { get; set; }
        int CurrentBet { get; set; }
        bool IsActive { get; set; }
        bool IsAllIn { get; }
        bool HasFolded { get; set; }
        List<Card> HoleCards { get; }
        int SeatPosition { get; set; }
        bool IsDealer { get; set; }
        bool IsSmallBlind { get; set; }
        bool IsBigBlind { get; set; }

        void AddCard(Card card);
        void ClearCards();
        void Bet(int amount);
        void Fold();
        void ResetForNewHand();
    }
}
