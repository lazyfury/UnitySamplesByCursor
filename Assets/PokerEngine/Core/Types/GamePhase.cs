namespace PokerEngine.Core.Types
{
    /// <summary>
    /// 游戏阶段
    /// </summary>
    public enum GamePhase
    {
        NotStarted,      // 未开始
        PreFlop,         // 翻牌前
        Flop,            // 翻牌
        Turn,            // 转牌
        River,           // 河牌
        Showdown,        // 摊牌
        Finished         // 结束
    }
}
