namespace InventoryEngine.Core.Interfaces
{
    /// <summary>
    /// Hotbar槽位接口（快捷栏槽位）
    /// </summary>
    public interface IHotbarSlot
    {
        /// <summary>
        /// 槽位索引（0-9，对应数字键）
        /// </summary>
        int Index { get; }
        
        /// <summary>
        /// 槽位中的物品（为空表示槽位为空）
        /// </summary>
        IItem Item { get; }
        
        /// <summary>
        /// 槽位中物品的数量（堆叠物品使用）
        /// </summary>
        int ItemCount { get; }
        
        /// <summary>
        /// 是否为空
        /// </summary>
        bool IsEmpty { get; }
        
        /// <summary>
        /// 关联的背包槽位索引（如果是从背包引用的）
        /// </summary>
        int? LinkedInventorySlotIndex { get; set; }
        
        /// <summary>
        /// 设置物品（从背包链接）
        /// </summary>
        /// <param name="item">要设置的物品</param>
        /// <param name="inventorySlotIndex">背包槽位索引</param>
        /// <returns>是否成功设置</returns>
        bool SetItem(IItem item, int? inventorySlotIndex = null);
        
        /// <summary>
        /// 清空槽位
        /// </summary>
        void Clear();
        
        /// <summary>
        /// 尝试使用物品（消耗一个）
        /// </summary>
        /// <returns>是否成功使用</returns>
        bool TryUse();
        
        /// <summary>
        /// 尝试使用指定数量的物品
        /// </summary>
        /// <param name="amount">要使用的数量</param>
        /// <returns>实际使用的数量</returns>
        int TryUse(int amount);
        
        /// <summary>
        /// 刷新物品信息（从背包同步）
        /// </summary>
        /// <param name="inventory">背包引用</param>
        /// <returns>是否成功刷新</returns>
        bool Refresh(IInventory inventory);
    }
}
