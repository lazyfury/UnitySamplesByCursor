using System.Collections.Generic;
using System.Linq;
using InventoryEngine.Core.Interfaces;

namespace InventoryEngine.Core.Implementations
{
    /// <summary>
    /// 背包实现类
    /// </summary>
    public class Inventory : IInventory
    {
        private readonly List<IInventorySlot> _slots;
        
        public int Capacity => _slots.Count;
        public int UsedSlots => _slots.Count(slot => !slot.IsEmpty);
        public int FreeSlots => Capacity - UsedSlots;
        public bool IsFull => FreeSlots == 0 && _slots.All(slot => slot.IsFull || slot.IsEmpty);
        
        public IEnumerable<IInventorySlot> Slots => _slots;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="capacity">背包容量</param>
        public Inventory(int capacity = 30)
        {
            if (capacity <= 0)
                capacity = 30;
                
            _slots = new List<IInventorySlot>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                _slots.Add(new InventorySlot(i));
            }
        }
        
        public IInventorySlot GetSlot(int index)
        {
            if (index < 0 || index >= Capacity)
                return null;
                
            return _slots[index];
        }
        
        public int AddItem(IItem item, int amount = 1)
        {
            if (item == null || amount <= 0)
                return 0;
                
            int remaining = amount;
            
            // 如果可以堆叠，先尝试在已有物品上堆叠
            if (item.CanStack)
            {
                var existingSlots = FindItemSlots(item.ItemConfigId);
                foreach (var slotIndex in existingSlots)
                {
                    if (remaining <= 0)
                        break;
                        
                    var slot = _slots[slotIndex];
                    int addable = slot.TryAddStack(remaining);
                    remaining -= addable;
                }
            }
            
            // 如果还有剩余，尝试放入空槽位
            while (remaining > 0)
            {
                var emptySlot = _slots.FirstOrDefault(slot => slot.IsEmpty);
                if (emptySlot == null)
                    break;
                    
                if (item.CanStack)
                {
                    int stackToAdd = System.Math.Min(remaining, item.MaxStackSize);
                    var newItem = item.Clone();
                    newItem.StackCount = stackToAdd;
                    
                    if (emptySlot.SetItem(newItem))
                    {
                        remaining -= stackToAdd;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    // 不可堆叠物品，每个槽位放一个
                    var newItem = item.Clone();
                    if (emptySlot.SetItem(newItem))
                    {
                        remaining -= 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            return amount - remaining;
        }
        
        public int RemoveItem(int itemId, int amount = 1)
        {
            if (amount <= 0)
                return 0;
                
            int removed = 0;
            var slots = FindItemSlots(itemId);
            
            foreach (var slotIndex in slots)
            {
                if (removed >= amount)
                    break;
                    
                var slot = _slots[slotIndex];
                int toRemove = amount - removed;
                removed += slot.TryRemoveStack(toRemove);
            }
            
            return removed;
        }
        
        public int RemoveItemAt(int slotIndex, int amount = 1)
        {
            var slot = GetSlot(slotIndex);
            if (slot == null || slot.IsEmpty)
                return 0;
                
            return slot.TryRemoveStack(amount);
        }
        
        public bool HasItem(int itemId, int amount = 1)
        {
            return GetItemCount(itemId) >= amount;
        }
        
        public int GetItemCount(int itemId)
        {
            int count = 0;
            foreach (var slot in _slots)
            {
                if (!slot.IsEmpty && slot.Item.ItemConfigId == itemId)
                {
                    count += slot.Item.StackCount;
                }
            }
            return count;
        }
        
        public List<int> FindItemSlots(int itemId)
        {
            var slots = new List<int>();
            for (int i = 0; i < _slots.Count; i++)
            {
                if (!_slots[i].IsEmpty && _slots[i].Item.ItemConfigId == itemId)
                {
                    slots.Add(i);
                }
            }
            return slots;
        }
        
        public void Clear()
        {
            foreach (var slot in _slots)
            {
                slot.Clear();
            }
        }
        
        public bool SwapSlots(int slotIndex1, int slotIndex2)
        {
            var slot1 = GetSlot(slotIndex1);
            var slot2 = GetSlot(slotIndex2);
            
            if (slot1 == null || slot2 == null)
                return false;
                
            return slot1.Swap(slot2);
        }
        
        public void Sort()
        {
            // 收集所有物品
            var allItems = new List<(IItem item, int stackCount)>();
            
            foreach (var slot in _slots)
            {
                if (!slot.IsEmpty)
                {
                    allItems.Add((slot.Item, slot.Item.StackCount));
                    slot.Clear();
                }
            }
            
            // 按类型和ID排序，并合并相同物品
            var grouped = allItems
                .GroupBy(x => x.item.ItemConfigId)
                .OrderBy(g => g.First().item.Type)
                .ThenBy(g => g.First().item.ItemConfigId);
            
            int slotIndex = 0;
            foreach (var group in grouped)
            {
                var item = group.First().item;
                int totalCount = group.Sum(x => x.stackCount);
                
                if (item.CanStack)
                {
                    // 可堆叠物品，按最大堆叠数分组
                    while (totalCount > 0 && slotIndex < Capacity)
                    {
                        var slot = _slots[slotIndex];
                        int stackToAdd = System.Math.Min(totalCount, item.MaxStackSize);
                        var newItem = item.Clone();
                        newItem.StackCount = stackToAdd;
                        
                        if (slot.SetItem(newItem))
                        {
                            totalCount -= stackToAdd;
                            slotIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    // 不可堆叠物品，每个占一个槽位
                    int itemCount = group.Count();
                    for (int i = 0; i < itemCount && slotIndex < Capacity; i++)
                    {
                        var slot = _slots[slotIndex];
                        var newItem = item.Clone();
                        if (slot.SetItem(newItem))
                        {
                            slotIndex++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
