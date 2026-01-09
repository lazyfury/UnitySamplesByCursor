namespace PokerEngine.Core.Types
{
    /// <summary>
    /// 手牌等级（从高到低）
    /// </summary>
    public enum HandRank
    {
        HighCard = 1,        // 高牌
        Pair = 2,            // 一对
        TwoPair = 3,         // 两对
        ThreeOfAKind = 4,    // 三条
        Straight = 5,        // 顺子
        Flush = 6,           // 同花
        FullHouse = 7,       // 葫芦
        FourOfAKind = 8,     // 四条
        StraightFlush = 9,   // 同花顺
        RoyalFlush = 10      // 皇家同花顺
    }
}
