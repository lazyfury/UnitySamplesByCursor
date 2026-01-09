namespace PokerEngine.Core.Types
{
    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameState
    {
        NotInitialized,  // 未初始化
        Waiting,         // 等待开始
        Playing,         // 游戏中
        Paused,          // 暂停
        Finished         // 结束
    }
}
