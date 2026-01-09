using System.Collections.Generic;
using System.Linq;
using InventoryEngine.Core.Interfaces;

namespace InventoryEngine.Core.Implementations
{
    /// <summary>
    /// 装备系统实现类
    /// </summary>
    public class Equipment : IEquipment
    {
        private readonly Dictionary<EquipmentSlotType, IEquipmentSlot> _slots;
        
        public IEnumerable<IEquipmentSlot> Slots => _slots.Values;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public Equipment()
        {
            _slots = new Dictionary<EquipmentSlotType, IEquipmentSlot>();
            
            // 初始化所有装备槽位
            var slotTypes = System.Enum.GetValues(typeof(EquipmentSlotType)) as EquipmentSlotType[];
            foreach (var slotType in slotTypes)
            {
                _slots[slotType] = new EquipmentSlot(slotType);
            }
        }
        
        /// <summary>
        /// 构造函数（只初始化指定的槽位类型）
        /// </summary>
        public Equipment(params EquipmentSlotType[] slotTypes)
        {
            _slots = new Dictionary<EquipmentSlotType, IEquipmentSlot>();
            
            foreach (var slotType in slotTypes)
            {
                _slots[slotType] = new EquipmentSlot(slotType);
            }
        }
        
        public IEquipmentSlot GetSlot(EquipmentSlotType slotType)
        {
            _slots.TryGetValue(slotType, out var slot);
            return slot;
        }
        
        public IItem EquipItem(IItem item, IInventory inventory)
        {
            if (item == null)
                return null;
            
            // 查找可以装备此物品的槽位
            var slotType = CanEquipItem(item);
            if (!slotType.HasValue)
                return null;
            
            var slot = GetSlot(slotType.Value);
            if (slot == null)
                return null;
            
            // 装备物品
            var oldItem = slot.Equip(item);
            
            // 如果有旧装备，尝试放入背包
            if (oldItem != null && inventory != null)
            {
                int added = inventory.AddItem(oldItem, 1);
                if (added == 0)
                {
                    // 背包满了，无法存放旧装备，恢复旧装备
                    slot.Equip(oldItem);
                    return null;
                }
            }
            
            return oldItem;
        }
        
        public bool Unequip(EquipmentSlotType slotType, IInventory inventory)
        {
            var slot = GetSlot(slotType);
            if (slot == null || !slot.IsEquipped)
                return false;
            
            var item = slot.Unequip();
            
            // 尝试将卸下的装备放入背包
            if (item != null && inventory != null)
            {
                int added = inventory.AddItem(item, 1);
                if (added == 0)
                {
                    // 背包满了，无法存放装备，恢复装备
                    slot.Equip(item);
                    return false;
                }
            }
            
            return true;
        }
        
        public EquipmentSlotType? CanEquipItem(IItem item)
        {
            if (item == null)
                return null;
            
            // 遍历所有槽位，查找可以装备此物品的槽位
            foreach (var kvp in _slots)
            {
                if (kvp.Value.CanEquip(item))
                {
                    return kvp.Key;
                }
            }
            
            return null;
        }
        
        public IEnumerable<IItem> GetEquippedItems()
        {
            return _slots.Values
                .Where(slot => slot.IsEquipped)
                .Select(slot => slot.Item);
        }
        
        public void ClearAll(IInventory inventory)
        {
            var equippedItems = GetEquippedItems().ToList();
            
            // 先清空所有槽位
            foreach (var slot in _slots.Values)
            {
                slot.Unequip();
            }
            
            // 将所有装备放入背包
            if (inventory != null)
            {
                foreach (var item in equippedItems)
                {
                    inventory.AddItem(item, 1);
                }
            }
        }
    }
}
