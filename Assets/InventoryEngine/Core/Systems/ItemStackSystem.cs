using System.Collections.Generic;
using InventoryEngine.Core.Interfaces;

namespace InventoryEngine.Core.Systems
{
    /// <summary>
    /// 物品堆叠系统，提供堆叠相关的工具方法
    /// </summary>
    public static class ItemStackSystem
    {
        /// <summary>
        /// 尝试将物品合并到现有槽位
        /// </summary>
        /// <param name="inventory">背包</param>
        /// <param name="item">要合并的物品</param>
        /// <returns>成功合并的数量</returns>
        public static int TryMergeStack(IInventory inventory, IItem item)
        {
            if (inventory == null || item == null || !item.CanStack)
                return 0;
                
            int merged = 0;
            var slots = inventory.FindItemSlots(item.ItemConfigId);
            
            foreach (var slotIndex in slots)
            {
                if (item.StackCount <= 0)
                    break;
                    
                var slot = inventory.GetSlot(slotIndex);
                if (slot != null && slot.Item.IsSameItem(item))
                {
                    int addable = slot.TryAddStack(item.StackCount);
                    merged += addable;
                    item.StackCount -= addable;
                }
            }
            
            return merged;
        }
        
        /// <summary>
        /// 拆分物品堆叠
        /// </summary>
        /// <param name="inventory">背包</param>
        /// <param name="slotIndex">源槽位索引</param>
        /// <param name="splitAmount">要拆分的数量</param>
        /// <param name="targetSlotIndex">目标槽位索引（-1表示第一个空槽位）</param>
        /// <returns>是否成功拆分</returns>
        public static bool SplitStack(IInventory inventory, int slotIndex, int splitAmount, int targetSlotIndex = -1)
        {
            if (inventory == null)
                return false;
                
            var sourceSlot = inventory.GetSlot(slotIndex);
            if (sourceSlot == null || sourceSlot.IsEmpty || !sourceSlot.Item.CanStack)
                return false;
                
            if (splitAmount <= 0 || splitAmount >= sourceSlot.Item.StackCount)
                return false;
                
            // 确定目标槽位
            IInventorySlot targetSlot = null;
            if (targetSlotIndex >= 0)
            {
                targetSlot = inventory.GetSlot(targetSlotIndex);
            }
            else
            {
                // 查找第一个空槽位
                foreach (var slot in inventory.Slots)
                {
                    if (slot.IsEmpty)
                    {
                        targetSlot = slot;
                        break;
                    }
                }
            }
            
            if (targetSlot == null || !targetSlot.IsEmpty)
                return false;
                
            // 拆分堆叠
            var newItem = sourceSlot.Item.Clone();
            newItem.StackCount = splitAmount;
            sourceSlot.Item.StackCount -= splitAmount;
            
            return targetSlot.SetItem(newItem);
        }
        
        /// <summary>
        /// 合并背包中所有相同的物品
        /// </summary>
        /// <param name="inventory">背包</param>
        /// <returns>合并的物品组数</returns>
        public static int MergeAllStacks(IInventory inventory)
        {
            if (inventory == null)
                return 0;
                
            // 使用背包的Sort方法会自动合并相同物品
            inventory.Sort();
            
            // 统计合并后的物品组数（相同配置的物品）
            var processedItems = new HashSet<int>();
            int mergedGroups = 0;
            
            foreach (var slot in inventory.Slots)
            {
                if (slot.IsEmpty)
                    continue;
                    
                int configId = slot.Item.ItemConfigId;
                if (!processedItems.Contains(configId))
                {
                    processedItems.Add(configId);
                    mergedGroups++;
                }
            }
            
            return mergedGroups;
        }
        
        /// <summary>
        /// 查找下一个空槽位
        /// </summary>
        private static int FindNextEmptySlot(IInventory inventory, int startIndex)
        {
            for (int i = startIndex; i < inventory.Capacity; i++)
            {
                var slot = inventory.GetSlot(i);
                if (slot != null && slot.IsEmpty)
                    return i;
            }
            return -1;
        }
    }
}
