namespace SGSKEngine.Core.Types
{
    /// <summary>
    /// 技能类型
    /// </summary>
    public enum SkillType
    {
        /// <summary>
        /// 主动技能（需要玩家主动使用）
        /// </summary>
        Active,
        
        /// <summary>
        /// 被动技能（自动触发）
        /// </summary>
        Passive,
        
        /// <summary>
        /// 限定技（整局游戏只能使用一次）
        /// </summary>
        Limited,
        
        /// <summary>
        /// 觉醒技（满足条件后自动获得）
        /// </summary>
        Awaken
    }
}
