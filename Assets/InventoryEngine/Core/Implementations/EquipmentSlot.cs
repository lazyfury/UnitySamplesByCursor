using InventoryEngine.Core.Interfaces;

namespace InventoryEngine.Core.Implementations
{
    /// <summary>
    /// 装备槽位实现类
    /// </summary>
    public class EquipmentSlot : IEquipmentSlot
    {
        public EquipmentSlotType SlotType { get; private set; }
        public IItem Item { get; private set; }
        
        public bool IsEquipped => Item != null;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public EquipmentSlot(EquipmentSlotType slotType)
        {
            SlotType = slotType;
            Item = null;
        }
        
        public IItem Equip(IItem item)
        {
            if (!CanEquip(item))
                return null;
            
            var oldItem = Item;
            Item = item;
            return oldItem;
        }
        
        public IItem Unequip()
        {
            var item = Item;
            Item = null;
            return item;
        }
        
        public bool CanEquip(IItem item)
        {
            if (item == null)
                return false;
            
            // 检查物品类型是否匹配槽位类型
            return SlotType.CanEquipItemType(item.Type);
        }
    }
}
