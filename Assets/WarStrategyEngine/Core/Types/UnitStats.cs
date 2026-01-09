namespace WarStrategyEngine.Core
{
    /// <summary>
    /// 单位属性
    /// </summary>
    public class UnitStats
    {
        /// <summary>
        /// 最大生命值
        /// </summary>
        public int MaxHealth { get; set; }
        
        /// <summary>
        /// 当前生命值
        /// </summary>
        public int CurrentHealth { get; set; }
        
        /// <summary>
        /// 攻击力
        /// </summary>
        public int Attack { get; set; }
        
        /// <summary>
        /// 防御力
        /// </summary>
        public int Defense { get; set; }
        
        /// <summary>
        /// 移动力
        /// </summary>
        public int Movement { get; set; }
        
        /// <summary>
        /// 攻击范围
        /// </summary>
        public int AttackRange { get; set; }
        
        /// <summary>
        /// 视野范围
        /// </summary>
        public int VisionRange { get; set; }
        
        /// <summary>
        /// 是否已移动（本回合）
        /// </summary>
        public bool HasMoved { get; set; }
        
        /// <summary>
        /// 是否已攻击（本回合）
        /// </summary>
        public bool HasAttacked { get; set; }
        
        public UnitStats()
        {
            MaxHealth = 100;
            CurrentHealth = 100;
            Attack = 10;
            Defense = 5;
            Movement = 2;
            AttackRange = 1;
            VisionRange = 3;
            HasMoved = false;
            HasAttacked = false;
        }
        
        public UnitStats(int maxHealth, int attack, int defense, int movement, int attackRange = 1, int visionRange = 3)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            Attack = attack;
            Defense = defense;
            Movement = movement;
            AttackRange = attackRange;
            VisionRange = visionRange;
            HasMoved = false;
            HasAttacked = false;
        }
        
        /// <summary>
        /// 重置回合状态
        /// </summary>
        public void ResetTurn()
        {
            HasMoved = false;
            HasAttacked = false;
        }
        
        /// <summary>
        /// 复制属性
        /// </summary>
        public UnitStats Clone()
        {
            return new UnitStats
            {
                MaxHealth = MaxHealth,
                CurrentHealth = CurrentHealth,
                Attack = Attack,
                Defense = Defense,
                Movement = Movement,
                AttackRange = AttackRange,
                VisionRange = VisionRange,
                HasMoved = HasMoved,
                HasAttacked = HasAttacked
            };
        }
    }
}
