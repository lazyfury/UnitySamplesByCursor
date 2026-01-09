namespace InventoryEngine.Core
{
    /// <summary>
    /// 物品类型枚举
    /// </summary>
    public enum ItemType
    {
        Weapon,         // 武器
        Armor,          // 防具
        Consumable,     // 消耗品
        Material,       // 材料
        Equipment,      // 装备
        Quest,          // 任务物品
        Misc            // 杂项
    }
    
    /// <summary>
    /// 物品类型扩展方法
    /// </summary>
    public static class ItemTypeExtensions
    {
        /// <summary>
        /// 判断物品类型是否可以堆叠
        /// </summary>
        public static bool CanStack(this ItemType type)
        {
            return type switch
            {
                ItemType.Consumable => true,
                ItemType.Material => true,
                ItemType.Quest => true,
                ItemType.Misc => true,
                _ => false
            };
        }
    }
}
