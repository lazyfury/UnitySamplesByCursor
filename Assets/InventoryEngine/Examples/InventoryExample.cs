using System;
using InventoryEngine.Core;
using InventoryEngine.Core.Interfaces;
using InventoryEngine.Core.Implementations;
using InventoryEngine.Core.Systems;

namespace InventoryEngine.Examples
{
    /// <summary>
    /// 背包引擎使用示例
    /// </summary>
    public class InventoryExample
    {
        /// <summary>
        /// 演示基本用法
        /// </summary>
        public static void BasicExample()
        {
            // 创建一个容量为30的背包
            IInventory inventory = new Inventory(30);
            
            // 创建一些物品
            IItem sword = new InventoryItem(
                itemConfigId: 1001,
                name: "铁剑",
                type: ItemType.Weapon,
                rarity: ItemRarity.Common,
                maxStackSize: 1,  // 武器不可堆叠
                description: "一把普通的铁剑",
                value: 50
            );
            
            IItem healthPotion = new InventoryItem(
                itemConfigId: 2001,
                name: "生命药水",
                type: ItemType.Consumable,
                rarity: ItemRarity.Common,
                maxStackSize: 10,  // 可以堆叠到10
                description: "恢复100点生命值",
                value: 25
            );
            
            IItem ironOre = new InventoryItem(
                itemConfigId: 3001,
                name: "铁矿",
                type: ItemType.Material,
                rarity: ItemRarity.Common,
                maxStackSize: 99,  // 可以堆叠到99
                description: "用于锻造的材料",
                value: 5
            );
            
            // 添加物品到背包
            Console.WriteLine("=== 添加物品 ===");
            int added1 = inventory.AddItem(sword, 1);
            Console.WriteLine($"添加 {sword.Name}: {added1} 个");
            
            int added2 = inventory.AddItem(healthPotion, 5);
            Console.WriteLine($"添加 {healthPotion.Name}: {added2} 个");
            
            int added3 = inventory.AddItem(ironOre, 25);
            Console.WriteLine($"添加 {ironOre.Name}: {added3} 个");
            
            // 检查背包状态
            Console.WriteLine("\n=== 背包状态 ===");
            Console.WriteLine($"容量: {inventory.Capacity}");
            Console.WriteLine($"已使用槽位: {inventory.UsedSlots}");
            Console.WriteLine($"空闲槽位: {inventory.FreeSlots}");
            Console.WriteLine($"是否已满: {inventory.IsFull}");
            
            // 查询物品数量
            Console.WriteLine("\n=== 物品数量 ===");
            Console.WriteLine($"{sword.Name}: {inventory.GetItemCount(sword.ItemConfigId)} 个");
            Console.WriteLine($"{healthPotion.Name}: {inventory.GetItemCount(healthPotion.ItemConfigId)} 个");
            Console.WriteLine($"{ironOre.Name}: {inventory.GetItemCount(ironOre.ItemConfigId)} 个");
            
            // 继续添加相同物品（会堆叠）
            Console.WriteLine("\n=== 继续添加相同物品 ===");
            int added4 = inventory.AddItem(healthPotion, 8);
            Console.WriteLine($"再次添加 {healthPotion.Name}: {added4} 个 (总共有 {inventory.GetItemCount(healthPotion.ItemConfigId)} 个)");
            
            // 移除物品
            Console.WriteLine("\n=== 移除物品 ===");
            int removed = inventory.RemoveItem(healthPotion.ItemConfigId, 7);
            Console.WriteLine($"移除 {healthPotion.Name}: {removed} 个 (剩余 {inventory.GetItemCount(healthPotion.ItemConfigId)} 个)");
            
            // 检查是否有物品
            Console.WriteLine("\n=== 检查物品 ===");
            Console.WriteLine($"是否有 {sword.Name}: {inventory.HasItem(sword.ItemConfigId)}");
            Console.WriteLine($"是否有10个 {healthPotion.Name}: {inventory.HasItem(healthPotion.ItemConfigId, 10)}");
            Console.WriteLine($"是否有5个 {healthPotion.Name}: {inventory.HasItem(healthPotion.ItemConfigId, 5)}");
        }
        
        /// <summary>
        /// 演示堆叠系统
        /// </summary>
        public static void StackingExample()
        {
            IInventory inventory = new Inventory(20);
            
            // 创建可堆叠物品
            IItem potion = new InventoryItem(
                itemConfigId: 2001,
                name: "药水",
                type: ItemType.Consumable,
                maxStackSize: 10
            );
            
            Console.WriteLine("=== 堆叠示例 ===");
            
            // 添加多个物品，它们会自动堆叠
            inventory.AddItem(potion, 25);  // 25个药水，应该占用3个槽位（10+10+5）
            
            Console.WriteLine($"添加25个药水");
            Console.WriteLine($"物品总数: {inventory.GetItemCount(potion.ItemConfigId)}");
            Console.WriteLine($"使用的槽位: {inventory.UsedSlots}");
            
            // 查找物品所在的槽位
            var slots = inventory.FindItemSlots(potion.ItemConfigId);
            Console.WriteLine($"物品所在槽位: {string.Join(", ", slots)}");
            
            // 查看每个槽位的堆叠情况
            foreach (var slotIndex in slots)
            {
                var slot = inventory.GetSlot(slotIndex);
                Console.WriteLine($"槽位 {slotIndex}: {slot.Item.StackCount} 个");
            }
            
            // 拆分堆叠
            Console.WriteLine("\n=== 拆分堆叠 ===");
            if (ItemStackSystem.SplitStack(inventory, slots[0], 3))
            {
                Console.WriteLine("成功从第一个槽位拆分出3个");
                Console.WriteLine($"现在使用的槽位: {inventory.UsedSlots}");
            }
        }
        
        /// <summary>
        /// 演示背包整理
        /// </summary>
        public static void SortExample()
        {
            IInventory inventory = new Inventory(15);
            
            // 添加不同类型的物品，顺序打乱
            inventory.AddItem(new InventoryItem(1001, "武器A", ItemType.Weapon), 1);
            inventory.AddItem(new InventoryItem(2001, "药水A", ItemType.Consumable, ItemRarity.Common, 10), 5);
            inventory.AddItem(new InventoryItem(3001, "材料A", ItemType.Material, ItemRarity.Common, 99), 20);
            inventory.AddItem(new InventoryItem(1002, "武器B", ItemType.Weapon), 1);
            inventory.AddItem(new InventoryItem(2001, "药水A", ItemType.Consumable, ItemRarity.Common, 10), 7);
            
            Console.WriteLine("=== 整理前 ===");
            PrintInventory(inventory);
            
            // 整理背包
            inventory.Sort();
            
            Console.WriteLine("\n=== 整理后 ===");
            PrintInventory(inventory);
        }
        
        /// <summary>
        /// 打印背包内容
        /// </summary>
        private static void PrintInventory(IInventory inventory)
        {
            int index = 0;
            foreach (var slot in inventory.Slots)
            {
                if (!slot.IsEmpty)
                {
                    var item = slot.Item;
                    string stackInfo = item.CanStack ? $"[{item.StackCount}]" : "";
                    Console.WriteLine($"槽位 {index}: {item.Name} {stackInfo} ({item.Type})");
                }
                index++;
            }
        }
    }
}
