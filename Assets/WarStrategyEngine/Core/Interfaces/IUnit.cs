using System;

namespace WarStrategyEngine.Core.Interfaces
{
    /// <summary>
    /// 单位接口
    /// </summary>
    public interface IUnit
    {
        /// <summary>
        /// 单位唯一ID
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// 单位类型
        /// </summary>
        UnitType Type { get; }
        
        /// <summary>
        /// 所属玩家ID
        /// </summary>
        int OwnerId { get; }
        
        /// <summary>
        /// 当前位置（地图坐标）
        /// </summary>
        Position Position { get; set; }
        
        /// <summary>
        /// 单位属性
        /// </summary>
        UnitStats Stats { get; }
        
        /// <summary>
        /// 是否存活
        /// </summary>
        bool IsAlive { get; }
        
        /// <summary>
        /// 移动单位到指定位置
        /// </summary>
        bool MoveTo(Position target);
        
        /// <summary>
        /// 攻击目标单位
        /// </summary>
        AttackResult Attack(IUnit target);
        
        /// <summary>
        /// 受到伤害
        /// </summary>
        void TakeDamage(int damage);
        
        /// <summary>
        /// 治疗单位
        /// </summary>
        void Heal(int amount);
    }
}
