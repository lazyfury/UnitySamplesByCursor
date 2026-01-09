namespace WarStrategyEngine.Core
{
    /// <summary>
    /// 单位类型枚举
    /// </summary>
    public enum UnitType
    {
        Infantry,      // 步兵
        Cavalry,       // 骑兵
        Archer,        // 弓箭手
        Siege,         // 攻城器械
        Naval,         // 海军
        Air,           // 空军
        Worker,        // 工人
        Hero           // 英雄单位
    }
    
    /// <summary>
    /// 单位类型扩展方法
    /// </summary>
    public static class UnitTypeExtensions
    {
        /// <summary>
        /// 获取单位类型的基础移动力
        /// </summary>
        public static int GetBaseMovement(this UnitType type)
        {
            return type switch
            {
                UnitType.Infantry => 2,
                UnitType.Cavalry => 4,
                UnitType.Archer => 2,
                UnitType.Siege => 1,
                UnitType.Naval => 3,
                UnitType.Air => 6,
                UnitType.Worker => 2,
                UnitType.Hero => 3,
                _ => 2
            };
        }
        
        /// <summary>
        /// 获取单位类型的攻击范围
        /// </summary>
        public static int GetAttackRange(this UnitType type)
        {
            return type switch
            {
                UnitType.Archer => 3,
                UnitType.Siege => 4,
                UnitType.Air => 2,
                _ => 1
            };
        }
    }
}
