using System.Collections.Generic;

namespace InventoryEngine.Core.Interfaces
{
    /// <summary>
    /// 装备系统接口
    /// </summary>
    public interface IEquipment
    {
        /// <summary>
        /// 获取所有装备槽位
        /// </summary>
        IEnumerable<IEquipmentSlot> Slots { get; }
        
        /// <summary>
        /// 根据装备槽位类型获取槽位
        /// </summary>
        IEquipmentSlot GetSlot(EquipmentSlotType slotType);
        
        /// <summary>
        /// 装备物品
        /// </summary>
        /// <param name="item">要装备的物品</param>
        /// <param name="inventory">背包引用（用于处理旧装备）</param>
        /// <returns>被替换的旧装备（如果有）</returns>
        IItem EquipItem(IItem item, IInventory inventory);
        
        /// <summary>
        /// 卸下装备
        /// </summary>
        /// <param name="slotType">装备槽位类型</param>
        /// <param name="inventory">背包引用（用于存放卸下的装备）</param>
        /// <returns>是否成功卸下</returns>
        bool Unequip(EquipmentSlotType slotType, IInventory inventory);
        
        /// <summary>
        /// 检查物品是否可以装备
        /// </summary>
        /// <param name="item">要检查的物品</param>
        /// <returns>可以装备的槽位类型（如果没有则返回null）</returns>
        EquipmentSlotType? CanEquipItem(IItem item);
        
        /// <summary>
        /// 获取已装备的物品列表
        /// </summary>
        IEnumerable<IItem> GetEquippedItems();
        
        /// <summary>
        /// 清空所有装备
        /// </summary>
        /// <param name="inventory">背包引用（用于存放卸下的装备）</param>
        void ClearAll(IInventory inventory);
    }
}
