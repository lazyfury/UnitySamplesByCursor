namespace SurvivorEngine.Core.Types
{
    /// <summary>
    /// 升级类型枚举
    /// </summary>
    public enum UpgradeType
    {
        // 属性升级
        MaxHealth,          // 最大生命值
        MoveSpeed,          // 移动速度
        Damage,             // 伤害
        AttackSpeed,        // 攻击速度
        AttackRange,        // 攻击范围
        ProjectileSpeed,    // 投射物速度
        ProjectileCount,    // 投射物数量
        CooldownReduction,  // 冷却缩减
        ExperienceGain,     // 经验获得
        PickupRange,        // 拾取范围
        
        // 武器升级
        NewWeapon,          // 新武器
        WeaponEvolution,    // 武器进化
        WeaponLevel         // 武器等级
    }
}
