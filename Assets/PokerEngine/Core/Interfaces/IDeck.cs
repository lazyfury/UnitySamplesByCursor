using System.Collections.Generic;
using PokerEngine.Core.Types;

namespace PokerEngine.Core.Interfaces
{
    /// <summary>
    /// 牌组接口
    /// </summary>
    public interface IDeck
    {
        int RemainingCards { get; }
        
        void Shuffle();
        Card DealCard();
        void Reset();
        List<Card> DealCards(int count);
    }
}
