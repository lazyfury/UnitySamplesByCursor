namespace WarStrategyEngine.Core
{
    /// <summary>
    /// 攻击结果
    /// </summary>
    public class AttackResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 造成的伤害
        /// </summary>
        public int Damage { get; set; }
        
        /// <summary>
        /// 目标是否被击杀
        /// </summary>
        public bool TargetKilled { get; set; }
        
        /// <summary>
        /// 攻击者是否反击
        /// </summary>
        public bool CounterAttacked { get; set; }
        
        /// <summary>
        /// 反击造成的伤害
        /// </summary>
        public int CounterDamage { get; set; }
        
        public AttackResult()
        {
            Success = false;
            Damage = 0;
            TargetKilled = false;
            CounterAttacked = false;
            CounterDamage = 0;
        }
    }
}
