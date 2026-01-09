namespace InventoryEngine.Core.Interfaces
{
    /// <summary>
    /// 背包槽位接口
    /// </summary>
    public interface IInventorySlot
    {
        /// <summary>
        /// 槽位索引
        /// </summary>
        int Index { get; }
        
        /// <summary>
        /// 槽位中的物品（为空表示槽位为空）
        /// </summary>
        IItem Item { get; }
        
        /// <summary>
        /// 是否为空
        /// </summary>
        bool IsEmpty { get; }
        
        /// <summary>
        /// 是否已满（堆叠物品达到最大数量）
        /// </summary>
        bool IsFull { get; }
        
        /// <summary>
        /// 放置物品
        /// </summary>
        /// <param name="item">要放置的物品</param>
        /// <returns>是否可以放置</returns>
        bool SetItem(IItem item);
        
        /// <summary>
        /// 移除物品
        /// </summary>
        /// <returns>移除的物品</returns>
        IItem RemoveItem();
        
        /// <summary>
        /// 清空槽位
        /// </summary>
        void Clear();
        
        /// <summary>
        /// 尝试添加堆叠数量
        /// </summary>
        /// <param name="amount">要添加的数量</param>
        /// <returns>实际添加的数量</returns>
        int TryAddStack(int amount);
        
        /// <summary>
        /// 尝试移除堆叠数量
        /// </summary>
        /// <param name="amount">要移除的数量</param>
        /// <returns>实际移除的数量</returns>
        int TryRemoveStack(int amount);
        
        /// <summary>
        /// 交换槽位内容
        /// </summary>
        /// <param name="other">另一个槽位</param>
        /// <returns>是否成功交换</returns>
        bool Swap(IInventorySlot other);
    }
}
