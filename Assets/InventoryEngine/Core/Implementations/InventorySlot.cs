using InventoryEngine.Core.Interfaces;

namespace InventoryEngine.Core.Implementations
{
    /// <summary>
    /// 背包槽位实现类
    /// </summary>
    public class InventorySlot : IInventorySlot
    {
        public int Index { get; private set; }
        public IItem Item { get; private set; }
        
        public bool IsEmpty => Item == null;
        public bool IsFull
        {
            get
            {
                if (IsEmpty || !Item.CanStack)
                    return !IsEmpty;
                    
                return Item.StackCount >= Item.MaxStackSize;
            }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public InventorySlot(int index)
        {
            Index = index;
            Item = null;
        }
        
        public bool SetItem(IItem item)
        {
            if (item == null)
            {
                Clear();
                return true;
            }
            
            if (!IsEmpty && !Item.IsSameItem(item))
            {
                // 槽位已有不同类型的物品
                return false;
            }
            
            if (IsEmpty)
            {
                // 空槽位，直接放置
                Item = item;
                return true;
            }
            else
            {
                // 相同物品，尝试合并堆叠
                if (Item.CanStack)
                {
                    int addable = System.Math.Min(
                        item.StackCount,
                        Item.MaxStackSize - Item.StackCount
                    );
                    
                    if (addable > 0)
                    {
                        Item.StackCount += addable;
                        return true;
                    }
                }
                
                return false;
            }
        }
        
        public IItem RemoveItem()
        {
            var item = Item;
            Item = null;
            return item;
        }
        
        public void Clear()
        {
            Item = null;
        }
        
        public int TryAddStack(int amount)
        {
            if (IsEmpty || amount <= 0)
                return 0;
                
            if (!Item.CanStack)
                return 0;
                
            int addable = System.Math.Min(amount, Item.MaxStackSize - Item.StackCount);
            Item.StackCount += addable;
            return addable;
        }
        
        public int TryRemoveStack(int amount)
        {
            if (IsEmpty || amount <= 0)
                return 0;
                
            if (!Item.CanStack)
            {
                // 不可堆叠物品，移除整个物品
                RemoveItem();
                return 1;
            }
            
            int removable = System.Math.Min(amount, Item.StackCount);
            Item.StackCount -= removable;
            
            if (Item.StackCount <= 0)
            {
                Clear();
            }
            
            return removable;
        }
        
        public bool Swap(IInventorySlot other)
        {
            if (other == null)
                return false;
                
            // 保存两个槽位的物品
            var thisItem = Item;
            var otherItem = other.Item;
            
            // 清空两个槽位
            Clear();
            other.Clear();
            
            // 交换物品
            bool thisSuccess = true;
            bool otherSuccess = true;
            
            if (thisItem != null)
            {
                otherSuccess = other.SetItem(thisItem);
            }
            
            if (otherItem != null)
            {
                thisSuccess = SetItem(otherItem);
            }
            
            return thisSuccess && otherSuccess;
        }
    }
}
