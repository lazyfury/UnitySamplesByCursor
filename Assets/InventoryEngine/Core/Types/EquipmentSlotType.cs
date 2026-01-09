namespace InventoryEngine.Core
{
    /// <summary>
    /// 装备槽位类型枚举
    /// </summary>
    public enum EquipmentSlotType
    {
        Weapon,         // 主武器
        OffHand,        // 副手（盾牌/副武器）
        Helmet,         // 头盔
        Armor,          // 护甲
        Gloves,         // 手套
        Boots,          // 靴子
        Ring,           // 戒指（可以有多个）
        Necklace,       // 项链
        Accessory       // 饰品
    }
    
    /// <summary>
    /// 装备槽位类型扩展方法
    /// </summary>
    public static class EquipmentSlotTypeExtensions
    {
        /// <summary>
        /// 判断装备槽位类型对应的物品类型是否匹配
        /// </summary>
        public static bool CanEquipItemType(this EquipmentSlotType slotType, ItemType itemType)
        {
            return slotType switch
            {
                EquipmentSlotType.Weapon => itemType == ItemType.Weapon,
                EquipmentSlotType.OffHand => itemType == ItemType.Weapon, // 副手也可以是武器
                EquipmentSlotType.Helmet => itemType == ItemType.Armor,
                EquipmentSlotType.Armor => itemType == ItemType.Armor,
                EquipmentSlotType.Gloves => itemType == ItemType.Armor,
                EquipmentSlotType.Boots => itemType == ItemType.Armor,
                EquipmentSlotType.Ring => itemType == ItemType.Equipment,
                EquipmentSlotType.Necklace => itemType == ItemType.Equipment,
                EquipmentSlotType.Accessory => itemType == ItemType.Equipment,
                _ => false
            };
        }
    }
}
