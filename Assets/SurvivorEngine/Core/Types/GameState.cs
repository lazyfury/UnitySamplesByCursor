namespace SurvivorEngine.Core.Types
{
    /// <summary>
    /// 游戏状态枚举
    /// </summary>
    public enum GameState
    {
        NotInitialized,  // 未初始化
        Menu,            // 菜单
        Playing,         // 游戏中
        LevelUp,         // 升级选择
        Paused,          // 暂停
        GameOver,        // 游戏结束
        Victory          // 胜利
    }
}
