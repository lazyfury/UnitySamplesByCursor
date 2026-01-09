namespace WarStrategyEngine.Core
{
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        NotInitialized,    // 未初始化
        Initializing,      // 初始化中
        WaitingForInput,   // 等待输入
        Processing,        // 处理中
        Paused,            // 暂停
        GameOver           // 游戏结束
    }
}
