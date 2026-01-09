using InventoryEngine.Core.Interfaces;

namespace InventoryEngine.Core.Implementations
{
    /// <summary>
    /// 物品实现类
    /// </summary>
    public class InventoryItem : IItem
    {
        private static int _nextId = 1;
        
        public int Id { get; private set; }
        public int ItemConfigId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ItemType Type { get; private set; }
        public ItemRarity Rarity { get; private set; }
        public string IconPath { get; private set; }
        public int MaxStackSize { get; private set; }
        public bool CanStack => MaxStackSize > 1;
        public int Value { get; private set; }
        
        private int _stackCount = 1;
        public int StackCount
        {
            get => _stackCount;
            set
            {
                if (CanStack)
                {
                    _stackCount = System.Math.Clamp(value, 1, MaxStackSize);
                }
                else
                {
                    _stackCount = 1;
                }
            }
        }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public InventoryItem(
            int itemConfigId,
            string name,
            ItemType type,
            ItemRarity rarity = ItemRarity.Common,
            int maxStackSize = 1,
            string description = "",
            string iconPath = "",
            int value = 0)
        {
            Id = _nextId++;
            ItemConfigId = itemConfigId;
            Name = name ?? "";
            Description = description ?? "";
            Type = type;
            Rarity = rarity;
            IconPath = iconPath ?? "";
            MaxStackSize = maxStackSize > 0 ? maxStackSize : 1;
            Value = value;
            _stackCount = 1;
            
            // 根据类型自动设置堆叠
            if (!Type.CanStack())
            {
                MaxStackSize = 1;
            }
        }
        
        /// <summary>
        /// 复制构造函数
        /// </summary>
        private InventoryItem(InventoryItem other)
        {
            Id = _nextId++;
            ItemConfigId = other.ItemConfigId;
            Name = other.Name;
            Description = other.Description;
            Type = other.Type;
            Rarity = other.Rarity;
            IconPath = other.IconPath;
            MaxStackSize = other.MaxStackSize;
            Value = other.Value;
            _stackCount = other._stackCount;
        }
        
        public IItem Clone()
        {
            return new InventoryItem(this);
        }
        
        public bool IsSameItem(IItem other)
        {
            if (other == null)
                return false;
                
            return ItemConfigId == other.ItemConfigId;
        }
        
        /// <summary>
        /// 创建指定数量的物品副本
        /// </summary>
        public static IItem CreateWithStack(IItem template, int stackCount)
        {
            if (template == null)
                return null;
                
            var cloned = template.Clone();
            cloned.StackCount = stackCount;
            return cloned;
        }
    }
}
