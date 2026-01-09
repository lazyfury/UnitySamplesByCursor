namespace WarStrategyEngine.Core
{
    /// <summary>
    /// 资源类型枚举
    /// </summary>
    public enum ResourceType
    {
        Gold,        // 金币
        Food,        // 食物
        Wood,        // 木材
        Stone,       // 石头
        Iron,        // 铁
        Mana         // 魔法值
    }
    
    /// <summary>
    /// 资源类
    /// </summary>
    public class Resource
    {
        public ResourceType Type { get; set; }
        public int Amount { get; set; }
        
        public Resource(ResourceType type, int amount = 0)
        {
            Type = type;
            Amount = amount;
        }
        
        public Resource Clone()
        {
            return new Resource(Type, Amount);
        }
    }
}
