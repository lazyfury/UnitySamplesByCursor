namespace InventoryEngine.Core
{
    /// <summary>
    /// 物品稀有度枚举
    /// </summary>
    public enum ItemRarity
    {
        Common,         // 普通
        Uncommon,       //  uncommon
        Rare,           // 稀有
        Epic,           // 史诗
        Legendary       // 传说
    }
    
    /// <summary>
    /// 物品稀有度扩展方法
    /// </summary>
    public static class ItemRarityExtensions
    {
        /// <summary>
        /// 获取稀有度对应的颜色值（十六进制）
        /// </summary>
        public static string GetColorHex(this ItemRarity rarity)
        {
            return rarity switch
            {
                ItemRarity.Common => "#FFFFFF",      // 白色
                ItemRarity.Uncommon => "#1EFF00",    // 绿色
                ItemRarity.Rare => "#0070DD",        // 蓝色
                ItemRarity.Epic => "#A335EE",        // 紫色
                ItemRarity.Legendary => "#FF8000",   // 橙色
                _ => "#FFFFFF"
            };
        }
    }
}
