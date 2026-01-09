namespace InventoryEngine.Core.Interfaces
{
    /// <summary>
    /// 装备槽位接口
    /// </summary>
    public interface IEquipmentSlot
    {
        /// <summary>
        /// 装备槽位类型
        /// </summary>
        EquipmentSlotType SlotType { get; }
        
        /// <summary>
        /// 槽位中的装备物品（为空表示未装备）
        /// </summary>
        IItem Item { get; }
        
        /// <summary>
        /// 是否已装备
        /// </summary>
        bool IsEquipped { get; }
        
        /// <summary>
        /// 装备物品
        /// </summary>
        /// <param name="item">要装备的物品</param>
        /// <returns>被替换的旧装备（如果有）</returns>
        IItem Equip(IItem item);
        
        /// <summary>
        /// 卸下装备
        /// </summary>
        /// <returns>卸下的装备物品</returns>
        IItem Unequip();
        
        /// <summary>
        /// 检查物品是否可以装备到此槽位
        /// </summary>
        /// <param name="item">要检查的物品</param>
        /// <returns>是否可以装备</returns>
        bool CanEquip(IItem item);
    }
}
