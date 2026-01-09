namespace InventoryEngine.Core.Interfaces
{
    /// <summary>
    /// 物品接口
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// 物品唯一ID
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// 物品配置ID（相同配置的物品可以有多个实例）
        /// </summary>
        int ItemConfigId { get; }
        
        /// <summary>
        /// 物品名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 物品描述
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// 物品类型
        /// </summary>
        ItemType Type { get; }
        
        /// <summary>
        /// 物品稀有度
        /// </summary>
        ItemRarity Rarity { get; }
        
        /// <summary>
        /// 物品图标资源路径（可选）
        /// </summary>
        string IconPath { get; }
        
        /// <summary>
        /// 最大堆叠数量（1表示不可堆叠）
        /// </summary>
        int MaxStackSize { get; }
        
        /// <summary>
        /// 是否可以堆叠
        /// </summary>
        bool CanStack { get; }
        
        /// <summary>
        /// 物品价值（用于交易等）
        /// </summary>
        int Value { get; }
        
        /// <summary>
        /// 当前堆叠数量（不可堆叠物品始终为1）
        /// </summary>
        int StackCount { get; set; }
        
        /// <summary>
        /// 创建物品的副本
        /// </summary>
        IItem Clone();
        
        /// <summary>
        /// 判断两个物品是否是相同配置
        /// </summary>
        bool IsSameItem(IItem other);
    }
}
