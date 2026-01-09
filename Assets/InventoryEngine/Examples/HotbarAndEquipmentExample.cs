using System;
using InventoryEngine.Core;
using InventoryEngine.Core.Interfaces;
using InventoryEngine.Core.Implementations;

namespace InventoryEngine.Examples
{
    /// <summary>
    /// Hotbar和装备系统使用示例
    /// </summary>
    public class HotbarAndEquipmentExample
    {
        /// <summary>
        /// 演示Hotbar（快捷栏）功能
        /// </summary>
        public static void HotbarExample()
        {
            // 创建背包和Hotbar
            IInventory inventory = new Inventory(30);
            IHotbar hotbar = new Hotbar(10); // 10个快捷栏槽位
            
            // 创建一些物品并添加到背包
            IItem healthPotion = new InventoryItem(
                itemConfigId: 2001,
                name: "生命药水",
                type: ItemType.Consumable,
                maxStackSize: 10
            );
            
            IItem manaPotion = new InventoryItem(
                itemConfigId: 2002,
                name: "魔法药水",
                type: ItemType.Consumable,
                maxStackSize: 10
            );
            
            IItem sword = new InventoryItem(
                itemConfigId: 1001,
                name: "铁剑",
                type: ItemType.Weapon
            );
            
            Console.WriteLine("=== Hotbar示例 ===");
            
            // 添加物品到背包
            inventory.AddItem(healthPotion, 5);
            inventory.AddItem(manaPotion, 8);
            inventory.AddItem(sword, 1);
            
            Console.WriteLine("\n添加物品到背包:");
            Console.WriteLine($"生命药水: {inventory.GetItemCount(healthPotion.ItemConfigId)} 个");
            Console.WriteLine($"魔法药水: {inventory.GetItemCount(manaPotion.ItemConfigId)} 个");
            Console.WriteLine($"铁剑: {inventory.GetItemCount(sword.ItemConfigId)} 个");
            
            // 查找背包中的物品槽位
            var healthPotionSlots = inventory.FindItemSlots(healthPotion.ItemConfigId);
            var manaPotionSlots = inventory.FindItemSlots(manaPotion.ItemConfigId);
            var swordSlots = inventory.FindItemSlots(sword.ItemConfigId);
            
            // 从背包添加到Hotbar
            Console.WriteLine("\n=== 添加物品到Hotbar ===");
            if (healthPotionSlots.Count > 0)
            {
                bool success = hotbar.AddFromInventory(inventory, healthPotionSlots[0], 0); // 添加到第0个槽位
                Console.WriteLine($"添加生命药水到Hotbar槽位0: {(success ? "成功" : "失败")}");
            }
            
            if (manaPotionSlots.Count > 0)
            {
                bool success = hotbar.AddFromInventory(inventory, manaPotionSlots[0], 1); // 添加到第1个槽位
                Console.WriteLine($"添加魔法药水到Hotbar槽位1: {(success ? "成功" : "失败")}");
            }
            
            if (swordSlots.Count > 0)
            {
                bool success = hotbar.AddFromInventory(inventory, swordSlots[0], 2); // 添加到第2个槽位
                Console.WriteLine($"添加铁剑到Hotbar槽位2: {(success ? "成功" : "失败")}");
            }
            
            // 显示Hotbar内容
            Console.WriteLine("\n=== Hotbar内容 ===");
            for (int i = 0; i < hotbar.Capacity; i++)
            {
                var slot = hotbar.GetSlot(i);
                if (!slot.IsEmpty)
                {
                    string countInfo = slot.Item.CanStack ? $" x{slot.ItemCount}" : "";
                    Console.WriteLine($"Hotbar槽位 {i}: {slot.Item.Name}{countInfo}");
                }
            }
            
            // 使用Hotbar中的物品
            Console.WriteLine("\n=== 使用Hotbar物品 ===");
            int used = hotbar.TryUse(0, 2); // 从槽位0使用2个
            Console.WriteLine($"从Hotbar槽位0使用了 {used} 个物品");
            
            // 刷新Hotbar（同步背包状态）
            hotbar.RefreshAll(inventory);
            
            Console.WriteLine("\n=== 刷新后的Hotbar内容 ===");
            for (int i = 0; i < hotbar.Capacity; i++)
            {
                var slot = hotbar.GetSlot(i);
                if (!slot.IsEmpty)
                {
                    string countInfo = slot.Item.CanStack ? $" x{slot.ItemCount}" : "";
                    Console.WriteLine($"Hotbar槽位 {i}: {slot.Item.Name}{countInfo}");
                }
            }
            
            // 交换Hotbar槽位
            Console.WriteLine("\n=== 交换Hotbar槽位 ===");
            hotbar.SwapSlots(0, 1);
            Console.WriteLine("交换槽位0和1");
            
            Console.WriteLine("\n=== 交换后的Hotbar内容 ===");
            for (int i = 0; i < hotbar.Capacity; i++)
            {
                var slot = hotbar.GetSlot(i);
                if (!slot.IsEmpty)
                {
                    string countInfo = slot.Item.CanStack ? $" x{slot.ItemCount}" : "";
                    Console.WriteLine($"Hotbar槽位 {i}: {slot.Item.Name}{countInfo}");
                }
            }
        }
        
        /// <summary>
        /// 演示装备系统功能
        /// </summary>
        public static void EquipmentExample()
        {
            // 创建背包和装备系统
            IInventory inventory = new Inventory(30);
            IEquipment equipment = new Equipment();
            
            // 创建装备物品
            IItem weapon = new InventoryItem(
                itemConfigId: 1001,
                name: "传奇之剑",
                type: ItemType.Weapon,
                rarity: ItemRarity.Legendary,
                description: "一把强大的传说级武器"
            );
            
            IItem helmet = new InventoryItem(
                itemConfigId: 3001,
                name: "钢铁头盔",
                type: ItemType.Armor,
                rarity: ItemRarity.Rare,
                description: "提供良好的头部防护"
            );
            
            IItem ring = new InventoryItem(
                itemConfigId: 4001,
                name: "力量戒指",
                type: ItemType.Equipment,
                rarity: ItemRarity.Epic,
                description: "增加力量属性"
            );
            
            IItem consumable = new InventoryItem(
                itemConfigId: 2001,
                name: "生命药水",
                type: ItemType.Consumable,
                maxStackSize: 10
            );
            
            Console.WriteLine("=== 装备系统示例 ===");
            
            // 添加物品到背包
            inventory.AddItem(weapon, 1);
            inventory.AddItem(helmet, 1);
            inventory.AddItem(ring, 1);
            inventory.AddItem(consumable, 5);
            
            Console.WriteLine("\n添加物品到背包:");
            Console.WriteLine($"传奇之剑: {inventory.GetItemCount(weapon.ItemConfigId)} 个");
            Console.WriteLine($"钢铁头盔: {inventory.GetItemCount(helmet.ItemConfigId)} 个");
            Console.WriteLine($"力量戒指: {inventory.GetItemCount(ring.ItemConfigId)} 个");
            
            // 装备物品
            Console.WriteLine("\n=== 装备物品 ===");
            
            // 从背包获取物品并装备
            var weaponSlots = inventory.FindItemSlots(weapon.ItemConfigId);
            if (weaponSlots.Count > 0)
            {
                var weaponSlot = inventory.GetSlot(weaponSlots[0]);
                var oldWeapon = equipment.EquipItem(weaponSlot.Item, inventory);
                if (oldWeapon == null)
                {
                    // 装备成功，从背包移除
                    inventory.RemoveItemAt(weaponSlots[0], 1);
                    Console.WriteLine($"装备: {weapon.Name}");
                }
                else
                {
                    Console.WriteLine($"装备: {weapon.Name}，卸下: {oldWeapon.Name}");
                }
            }
            
            var helmetSlots = inventory.FindItemSlots(helmet.ItemConfigId);
            if (helmetSlots.Count > 0)
            {
                var helmetSlot = inventory.GetSlot(helmetSlots[0]);
                var oldHelmet = equipment.EquipItem(helmetSlot.Item, inventory);
                if (oldHelmet == null)
                {
                    inventory.RemoveItemAt(helmetSlots[0], 1);
                    Console.WriteLine($"装备: {helmet.Name}");
                }
                else
                {
                    Console.WriteLine($"装备: {helmet.Name}，卸下: {oldHelmet.Name}");
                }
            }
            
            var ringSlots = inventory.FindItemSlots(ring.ItemConfigId);
            if (ringSlots.Count > 0)
            {
                var ringSlot = inventory.GetSlot(ringSlots[0]);
                var oldRing = equipment.EquipItem(ringSlot.Item, inventory);
                if (oldRing == null)
                {
                    inventory.RemoveItemAt(ringSlots[0], 1);
                    Console.WriteLine($"装备: {ring.Name}");
                }
                else
                {
                    Console.WriteLine($"装备: {ring.Name}，卸下: {oldRing.Name}");
                }
            }
            
            // 尝试装备不可装备的物品（消耗品）
            var consumableSlots = inventory.FindItemSlots(consumable.ItemConfigId);
            if (consumableSlots.Count > 0)
            {
                var consumableSlot = inventory.GetSlot(consumableSlots[0]);
                var canEquip = equipment.CanEquipItem(consumableSlot.Item);
                Console.WriteLine($"尝试装备消耗品: {(canEquip.HasValue ? "成功" : "失败（消耗品不可装备）")}");
            }
            
            // 显示已装备的物品
            Console.WriteLine("\n=== 已装备的物品 ===");
            foreach (var item in equipment.GetEquippedItems())
            {
                var slotType = equipment.CanEquipItem(item);
                Console.WriteLine($"{item.Name} - 槽位: {slotType}");
            }
            
            // 查看各个装备槽位
            Console.WriteLine("\n=== 装备槽位详情 ===");
            var weaponSlot_eq = equipment.GetSlot(EquipmentSlotType.Weapon);
            if (weaponSlot_eq != null && weaponSlot_eq.IsEquipped)
            {
                Console.WriteLine($"武器槽位: {weaponSlot_eq.Item.Name}");
            }
            
            var helmetSlot_eq = equipment.GetSlot(EquipmentSlotType.Helmet);
            if (helmetSlot_eq != null && helmetSlot_eq.IsEquipped)
            {
                Console.WriteLine($"头盔槽位: {helmetSlot_eq.Item.Name}");
            }
            
            var ringSlot_eq = equipment.GetSlot(EquipmentSlotType.Ring);
            if (ringSlot_eq != null && ringSlot_eq.IsEquipped)
            {
                Console.WriteLine($"戒指槽位: {ringSlot_eq.Item.Name}");
            }
            
            // 卸下装备
            Console.WriteLine("\n=== 卸下装备 ===");
            bool unequipped = equipment.Unequip(EquipmentSlotType.Weapon, inventory);
            if (unequipped)
            {
                Console.WriteLine("成功卸下武器，已放入背包");
                Console.WriteLine($"背包中的传奇之剑: {inventory.GetItemCount(weapon.ItemConfigId)} 个");
            }
            
            // 装备更好的武器
            IItem betterWeapon = new InventoryItem(
                itemConfigId: 1002,
                name: "神之剑",
                type: ItemType.Weapon,
                rarity: ItemRarity.Legendary,
                description: "更强大的传说级武器"
            );
            
            inventory.AddItem(betterWeapon, 1);
            var betterWeaponSlots = inventory.FindItemSlots(betterWeapon.ItemConfigId);
            if (betterWeaponSlots.Count > 0)
            {
                var betterWeaponSlot = inventory.GetSlot(betterWeaponSlots[0]);
                var oldWeapon = equipment.EquipItem(betterWeaponSlot.Item, inventory);
                if (oldWeapon == null)
                {
                    inventory.RemoveItemAt(betterWeaponSlots[0], 1);
                    Console.WriteLine($"装备新武器: {betterWeapon.Name}");
                }
                else
                {
                    Console.WriteLine($"装备新武器: {betterWeapon.Name}，旧武器 {oldWeapon.Name} 已放入背包");
                }
            }
        }
        
        /// <summary>
        /// 演示Hotbar和装备系统的结合使用
        /// </summary>
        public static void CombinedExample()
        {
            IInventory inventory = new Inventory(30);
            IHotbar hotbar = new Hotbar(10);
            IEquipment equipment = new Equipment();
            
            // 创建物品
            IItem healthPotion = new InventoryItem(2001, "生命药水", ItemType.Consumable, ItemRarity.Common, 10);
            IItem manaPotion = new InventoryItem(2002, "魔法药水", ItemType.Consumable, ItemRarity.Common, 10);
            IItem sword = new InventoryItem(1001, "铁剑", ItemType.Weapon);
            IItem helmet = new InventoryItem(3001, "头盔", ItemType.Armor);
            
            Console.WriteLine("=== Hotbar和装备系统结合示例 ===");
            
            // 添加物品
            inventory.AddItem(healthPotion, 5);
            inventory.AddItem(manaPotion, 3);
            inventory.AddItem(sword, 1);
            inventory.AddItem(helmet, 1);
            
            // 装备物品
            var swordSlot = inventory.FindItemSlots(sword.ItemConfigId);
            if (swordSlot.Count > 0)
            {
                var slot = inventory.GetSlot(swordSlot[0]);
                equipment.EquipItem(slot.Item, inventory);
                inventory.RemoveItemAt(swordSlot[0], 1);
                Console.WriteLine("装备了武器");
            }
            
            // 添加消耗品到Hotbar
            var potionSlot = inventory.FindItemSlots(healthPotion.ItemConfigId);
            if (potionSlot.Count > 0)
            {
                hotbar.AddFromInventory(inventory, potionSlot[0], 0);
                Console.WriteLine("添加生命药水到Hotbar");
            }
            
            // 显示状态
            Console.WriteLine("\n=== 当前状态 ===");
            Console.WriteLine($"背包容量: {inventory.UsedSlots}/{inventory.Capacity}");
            Console.WriteLine($"已装备物品数: {System.Linq.Enumerable.Count(equipment.GetEquippedItems())}");
            
            int hotbarUsed = 0;
            for (int i = 0; i < hotbar.Capacity; i++)
            {
                if (!hotbar.GetSlot(i).IsEmpty)
                    hotbarUsed++;
            }
            Console.WriteLine($"Hotbar使用: {hotbarUsed}/{hotbar.Capacity}");
        }
    }
}
