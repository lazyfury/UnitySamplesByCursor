namespace Match3Engine.Core.Types
{
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        NotInitialized,  // 未初始化
        Ready,           // 准备就绪
        WaitingForInput, // 等待输入
        Swapping,        // 交换中
        Checking,        // 检查匹配
        Eliminating,     // 消除中
        Falling,         // 下落中
        Filling,         // 填充中
        Paused,          // 暂停
        GameOver         // 游戏结束
    }
}
