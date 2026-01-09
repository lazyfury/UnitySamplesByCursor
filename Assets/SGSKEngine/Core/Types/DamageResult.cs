namespace SGSKEngine.Core.Types
{
    /// <summary>
    /// 伤害结果
    /// </summary>
    public class DamageResult
    {
        /// <summary>
        /// 伤害来源
        /// </summary>
        public int SourceId { get; set; }
        
        /// <summary>
        /// 伤害目标
        /// </summary>
        public int TargetId { get; set; }
        
        /// <summary>
        /// 伤害值
        /// </summary>
        public int Amount { get; set; }
        
        /// <summary>
        /// 伤害类型
        /// </summary>
        public DamageType Type { get; set; }
        
        /// <summary>
        /// 是否被闪避
        /// </summary>
        public bool Dodged { get; set; }
        
        /// <summary>
        /// 是否造成实际伤害
        /// </summary>
        public bool Dealt => !Dodged && Amount > 0;
    }
}
