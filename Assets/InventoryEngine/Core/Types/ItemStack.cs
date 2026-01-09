namespace InventoryEngine.Core
{
    /// <summary>
    /// 物品堆叠信息结构
    /// </summary>
    public struct ItemStack
    {
        /// <summary>
        /// 最大堆叠数量
        /// </summary>
        public int MaxStackSize { get; set; }
        
        /// <summary>
        /// 当前堆叠数量
        /// </summary>
        public int CurrentStack { get; set; }
        
        /// <summary>
        /// 是否可以堆叠
        /// </summary>
        public bool CanStack => MaxStackSize > 1;
        
        /// <summary>
        /// 是否已满
        /// </summary>
        public bool IsFull => CurrentStack >= MaxStackSize;
        
        /// <summary>
        /// 剩余可堆叠数量
        /// </summary>
        public int RemainingSpace => MaxStackSize - CurrentStack;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public ItemStack(int maxStackSize, int currentStack = 0)
        {
            MaxStackSize = maxStackSize > 0 ? maxStackSize : 1;
            CurrentStack = currentStack > MaxStackSize ? MaxStackSize : (currentStack < 0 ? 0 : currentStack);
        }
        
        /// <summary>
        /// 尝试添加数量
        /// </summary>
        /// <returns>实际添加的数量</returns>
        public int TryAdd(int amount)
        {
            if (!CanStack || amount <= 0)
                return 0;
                
            int addable = System.Math.Min(amount, RemainingSpace);
            CurrentStack += addable;
            return addable;
        }
        
        /// <summary>
        /// 尝试移除数量
        /// </summary>
        /// <returns>实际移除的数量</returns>
        public int TryRemove(int amount)
        {
            if (amount <= 0)
                return 0;
                
            int removable = System.Math.Min(amount, CurrentStack);
            CurrentStack -= removable;
            return removable;
        }
        
        /// <summary>
        /// 设置为空
        /// </summary>
        public void Clear()
        {
            CurrentStack = 0;
        }
    }
}
