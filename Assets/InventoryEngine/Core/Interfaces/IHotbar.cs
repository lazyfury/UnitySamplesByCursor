using System.Collections.Generic;

namespace InventoryEngine.Core.Interfaces
{
    /// <summary>
    /// Hotbar接口（快捷栏）
    /// </summary>
    public interface IHotbar
    {
        /// <summary>
        /// Hotbar容量（通常为10，对应数字键1-9和0）
        /// </summary>
        int Capacity { get; }
        
        /// <summary>
        /// 获取所有槽位
        /// </summary>
        IEnumerable<IHotbarSlot> Slots { get; }
        
        /// <summary>
        /// 根据索引获取槽位
        /// </summary>
        IHotbarSlot GetSlot(int index);
        
        /// <summary>
        /// 从背包添加物品到Hotbar
        /// </summary>
        /// <param name="inventorySlotIndex">背包槽位索引</param>
        /// <param name="hotbarSlotIndex">Hotbar槽位索引（-1表示第一个空槽位）</param>
        /// <returns>是否成功添加</returns>
        bool AddFromInventory(IInventory inventory, int inventorySlotIndex, int hotbarSlotIndex = -1);
        
        /// <summary>
        /// 从Hotbar移除物品
        /// </summary>
        /// <param name="hotbarSlotIndex">Hotbar槽位索引</param>
        /// <returns>是否成功移除</returns>
        bool Remove(int hotbarSlotIndex);
        
        /// <summary>
        /// 清空Hotbar
        /// </summary>
        void Clear();
        
        /// <summary>
        /// 交换两个槽位的内容
        /// </summary>
        /// <param name="slotIndex1">槽位1索引</param>
        /// <param name="slotIndex2">槽位2索引</param>
        /// <returns>是否成功交换</returns>
        bool SwapSlots(int slotIndex1, int slotIndex2);
        
        /// <summary>
        /// 刷新所有槽位（从背包同步物品信息）
        /// </summary>
        /// <param name="inventory">背包引用</param>
        void RefreshAll(IInventory inventory);
        
        /// <summary>
        /// 尝试使用指定槽位的物品
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="amount">要使用的数量</param>
        /// <returns>实际使用的数量</returns>
        int TryUse(int slotIndex, int amount = 1);
    }
}
