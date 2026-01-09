using System.Collections.Generic;
using System.Linq;
using InventoryEngine.Core.Interfaces;

namespace InventoryEngine.Core.Implementations
{
    /// <summary>
    /// Hotbar实现类（快捷栏）
    /// </summary>
    public class Hotbar : IHotbar
    {
        private readonly List<IHotbarSlot> _slots;
        
        public int Capacity => _slots.Count;
        
        public IEnumerable<IHotbarSlot> Slots => _slots;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="capacity">Hotbar容量（默认10）</param>
        public Hotbar(int capacity = 10)
        {
            if (capacity <= 0)
                capacity = 10;
                
            _slots = new List<IHotbarSlot>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                _slots.Add(new HotbarSlot(i));
            }
        }
        
        public IHotbarSlot GetSlot(int index)
        {
            if (index < 0 || index >= Capacity)
                return null;
                
            return _slots[index];
        }
        
        public bool AddFromInventory(IInventory inventory, int inventorySlotIndex, int hotbarSlotIndex = -1)
        {
            if (inventory == null)
                return false;
                
            var inventorySlot = inventory.GetSlot(inventorySlotIndex);
            if (inventorySlot == null || inventorySlot.IsEmpty)
                return false;
            
            // 确定目标Hotbar槽位
            IHotbarSlot targetSlot = null;
            if (hotbarSlotIndex >= 0)
            {
                targetSlot = GetSlot(hotbarSlotIndex);
                if (targetSlot == null)
                    return false;
            }
            else
            {
                // 查找第一个空槽位
                targetSlot = _slots.FirstOrDefault(slot => slot.IsEmpty);
                if (targetSlot == null)
                    return false;
            }
            
            // 设置物品并关联背包槽位
            return targetSlot.SetItem(inventorySlot.Item, inventorySlotIndex);
        }
        
        public bool Remove(int hotbarSlotIndex)
        {
            var slot = GetSlot(hotbarSlotIndex);
            if (slot == null)
                return false;
                
            slot.Clear();
            return true;
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
            
            // 交换物品和关联的背包槽位
            var tempItem = slot1.Item;
            var tempInventoryIndex = slot1.LinkedInventorySlotIndex;
            
            slot1.SetItem(slot2.Item, slot2.LinkedInventorySlotIndex);
            slot2.SetItem(tempItem, tempInventoryIndex);
            
            return true;
        }
        
        public void RefreshAll(IInventory inventory)
        {
            if (inventory == null)
                return;
                
            foreach (var slot in _slots)
            {
                slot.Refresh(inventory);
            }
        }
        
        public int TryUse(int slotIndex, int amount = 1)
        {
            var slot = GetSlot(slotIndex);
            if (slot == null)
                return 0;
                
            int used = slot.TryUse(amount);
            
            // 如果使用了物品，需要同步到背包
            if (used > 0 && slot.LinkedInventorySlotIndex.HasValue)
            {
                var inventory = slot.Item != null ? slot.Item : null;
                // 这里需要背包引用才能同步，但接口中没有，所以使用Refresh方法
                // 实际使用中，应该在外部调用RefreshAll来同步
            }
            
            return used;
        }
    }
}
