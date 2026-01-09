using System.Collections.Generic;

namespace InventoryEngine.Core.Interfaces
{
    /// <summary>
    /// 背包接口
    /// </summary>
    public interface IInventory
    {
        /// <summary>
        /// 背包容量（槽位数量）
        /// </summary>
        int Capacity { get; }
        
        /// <summary>
        /// 当前已使用的槽位数量
        /// </summary>
        int UsedSlots { get; }
        
        /// <summary>
        /// 空闲槽位数量
        /// </summary>
        int FreeSlots { get; }
        
        /// <summary>
        /// 是否已满
        /// </summary>
        bool IsFull { get; }
        
        /// <summary>
        /// 获取所有槽位
        /// </summary>
        IEnumerable<IInventorySlot> Slots { get; }
        
        /// <summary>
        /// 根据索引获取槽位
        /// </summary>
        IInventorySlot GetSlot(int index);
        
        /// <summary>
        /// 添加物品到背包
        /// </summary>
        /// <param name="item">要添加的物品</param>
        /// <param name="amount">数量（堆叠物品使用）</param>
        /// <returns>实际添加的数量</returns>
        int AddItem(IItem item, int amount = 1);
        
        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="amount">要移除的数量</param>
        /// <returns>实际移除的数量</returns>
        int RemoveItem(int itemId, int amount = 1);
        
        /// <summary>
        /// 移除指定槽位的物品
        /// </summary>
        /// <param name="slotIndex">槽位索引</param>
        /// <param name="amount">要移除的数量</param>
        /// <returns>实际移除的数量</returns>
        int RemoveItemAt(int slotIndex, int amount = 1);
        
        /// <summary>
        /// 检查背包中是否有指定物品
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <param name="amount">所需数量</param>
        /// <returns>是否有足够的物品</returns>
        bool HasItem(int itemId, int amount = 1);
        
        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>物品总数量</returns>
        int GetItemCount(int itemId);
        
        /// <summary>
        /// 查找物品所在的槽位
        /// </summary>
        /// <param name="itemId">物品ID</param>
        /// <returns>槽位索引列表</returns>
        List<int> FindItemSlots(int itemId);
        
        /// <summary>
        /// 清空背包
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
        /// 整理背包（自动排序和合并相同物品）
        /// </summary>
        void Sort();
    }
}
