using InventoryEngine.Core.Interfaces;

namespace InventoryEngine.Core.Implementations
{
    /// <summary>
    /// Hotbar槽位实现类
    /// </summary>
    public class HotbarSlot : IHotbarSlot
    {
        public int Index { get; private set; }
        public IItem Item { get; private set; }
        
        public int ItemCount => Item?.StackCount ?? 0;
        public bool IsEmpty => Item == null;
        public int? LinkedInventorySlotIndex { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public HotbarSlot(int index)
        {
            Index = index;
            Item = null;
            LinkedInventorySlotIndex = null;
        }
        
        public bool SetItem(IItem item, int? inventorySlotIndex = null)
        {
            if (item == null)
            {
                Clear();
                return true;
            }
            
            Item = item;
            LinkedInventorySlotIndex = inventorySlotIndex;
            return true;
        }
        
        public void Clear()
        {
            Item = null;
            LinkedInventorySlotIndex = null;
        }
        
        public bool TryUse()
        {
            return TryUse(1) > 0;
        }
        
        public int TryUse(int amount)
        {
            if (IsEmpty || amount <= 0)
                return 0;
            
            // 如果物品不可堆叠或数量不足，使用整个物品
            if (!Item.CanStack || Item.StackCount <= amount)
            {
                var item = Item;
                Clear();
                return item.StackCount;
            }
            
            // 减少堆叠数量
            int used = System.Math.Min(amount, Item.StackCount);
            Item.StackCount -= used;
            
            // 如果堆叠数量为0，清空槽位
            if (Item.StackCount <= 0)
            {
                Clear();
            }
            
            return used;
        }
        
        public bool Refresh(IInventory inventory)
        {
            if (inventory == null)
                return false;
            
            // 如果没有关联的背包槽位，无法刷新
            if (!LinkedInventorySlotIndex.HasValue)
            {
                // 如果槽位中的物品数量为0，清空槽位
                if (Item != null && Item.StackCount <= 0)
                {
                    Clear();
                }
                return true;
            }
            
            var inventorySlot = inventory.GetSlot(LinkedInventorySlotIndex.Value);
            if (inventorySlot == null || inventorySlot.IsEmpty)
            {
                // 关联的背包槽位为空，清空Hotbar槽位
                Clear();
                return true;
            }
            
            // 同步物品信息
            Item = inventorySlot.Item;
            return true;
        }
    }
}
