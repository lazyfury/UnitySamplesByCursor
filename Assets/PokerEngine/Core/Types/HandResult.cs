using System.Collections.Generic;

namespace PokerEngine.Core.Types
{
    /// <summary>
    /// 手牌评估结果
    /// </summary>
    public class HandResult
    {
        public HandRank Rank { get; }
        public List<Card> Cards { get; } // 组成手牌的5张牌
        public int Score { get; } // 用于比较的分数

        public HandResult(HandRank rank, List<Card> cards, int score)
        {
            Rank = rank;
            Cards = cards;
            Score = score;
        }
    }
}
