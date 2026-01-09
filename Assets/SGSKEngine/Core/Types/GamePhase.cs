namespace SGSKEngine.Core.Types
{
    /// <summary>
    /// 游戏阶段
    /// </summary>
    public enum GamePhase
    {
        /// <summary>
        /// 准备阶段
        /// </summary>
        Prepare,
        
        /// <summary>
        /// 判定阶段
        /// </summary>
        Judgment,
        
        /// <summary>
        /// 摸牌阶段
        /// </summary>
        Draw,
        
        /// <summary>
        /// 出牌阶段
        /// </summary>
        Play,
        
        /// <summary>
        /// 弃牌阶段
        /// </summary>
        Discard,
        
        /// <summary>
        /// 结束阶段
        /// </summary>
        Finish
    }
}
