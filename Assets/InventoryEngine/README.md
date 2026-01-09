# 背包引擎 (Inventory Engine)

一个简单通用的背包系统，采用接口-实现分离的架构设计，核心逻辑与Unity渲染层解耦，便于测试和扩展。

## 核心特性

- **物品管理** - 支持多种物品类型（武器、防具、消耗品、材料等）
- **堆叠系统** - 支持物品自动堆叠，可配置最大堆叠数量
- **稀有度系统** - 支持物品稀有度等级（普通、uncommon、稀有、史诗、传说）
- **容量管理** - 支持自定义背包容量
- **自动整理** - 支持背包自动排序和合并相同物品
- **Hotbar（快捷栏）** - 支持快捷栏，快速访问常用物品
- **装备系统** - 支持装备不同类型的物品（武器、防具、饰品等）
- **类型安全** - 使用接口-实现分离，便于扩展和测试

## 目录结构

```
InventoryEngine/
├── Core/
│   ├── Types/              # 类型定义
│   │   ├── ItemType.cs     # 物品类型枚举
│   │   ├── ItemRarity.cs   # 物品稀有度枚举
│   │   ├── ItemStack.cs    # 物品堆叠结构
│   │   └── EquipmentSlotType.cs  # 装备槽位类型枚举
│   │
│   ├── Interfaces/         # 接口定义
│   │   ├── IItem.cs        # 物品接口
│   │   ├── IInventory.cs   # 背包接口
│   │   ├── IInventorySlot.cs  # 背包槽位接口
│   │   ├── IHotbar.cs      # Hotbar（快捷栏）接口
│   │   ├── IHotbarSlot.cs  # Hotbar槽位接口
│   │   ├── IEquipment.cs   # 装备系统接口
│   │   └── IEquipmentSlot.cs  # 装备槽位接口
│   │
│   ├── Implementations/    # 核心实现
│   │   ├── InventoryItem.cs   # 物品实现
│   │   ├── Inventory.cs       # 背包实现
│   │   ├── InventorySlot.cs   # 背包槽位实现
│   │   ├── Hotbar.cs          # Hotbar实现
│   │   ├── HotbarSlot.cs      # Hotbar槽位实现
│   │   ├── Equipment.cs       # 装备系统实现
│   │   └── EquipmentSlot.cs   # 装备槽位实现
│   │
│   └── Systems/            # 系统模块
│       └── ItemStackSystem.cs  # 堆叠管理系统
│
└── Examples/
    ├── InventoryExample.cs          # 背包使用示例
    └── HotbarAndEquipmentExample.cs  # Hotbar和装备系统示例
```

## 快速开始

### 1. 创建背包

```csharp
using InventoryEngine.Core.Interfaces;
using InventoryEngine.Core.Implementations;

// 创建一个容量为30的背包
IInventory inventory = new Inventory(30);
```

### 2. 创建物品

```csharp
using InventoryEngine.Core;

// 创建不可堆叠物品（如武器）
IItem sword = new InventoryItem(
    itemConfigId: 1001,
    name: "铁剑",
    type: ItemType.Weapon,
    rarity: ItemRarity.Common,
    maxStackSize: 1,  // 武器不可堆叠
    description: "一把普通的铁剑",
    value: 50
);

// 创建可堆叠物品（如消耗品）
IItem healthPotion = new InventoryItem(
    itemConfigId: 2001,
    name: "生命药水",
    type: ItemType.Consumable,
    rarity: ItemRarity.Common,
    maxStackSize: 10,  // 可以堆叠到10
    description: "恢复100点生命值",
    value: 25
);
```

### 3. 添加物品

```csharp
// 添加单个物品
inventory.AddItem(sword, 1);

// 添加多个可堆叠物品（会自动堆叠）
inventory.AddItem(healthPotion, 25);  // 25个药水会自动分配到多个槽位
```

### 4. 查询物品

```csharp
// 检查是否有物品
bool hasSword = inventory.HasItem(sword.ItemConfigId);
bool hasEnoughPotions = inventory.HasItem(healthPotion.ItemConfigId, 10);

// 获取物品数量
int potionCount = inventory.GetItemCount(healthPotion.ItemConfigId);

// 查找物品所在的槽位
List<int> slots = inventory.FindItemSlots(healthPotion.ItemConfigId);
```

### 5. 移除物品

```csharp
// 移除指定数量的物品
int removed = inventory.RemoveItem(healthPotion.ItemConfigId, 5);

// 从指定槽位移除物品
int removed = inventory.RemoveItemAt(slotIndex, 3);
```

### 6. 整理背包

```csharp
// 自动排序和合并相同物品
inventory.Sort();

// 使用堆叠系统合并所有相同物品
using InventoryEngine.Core.Systems;
int mergedGroups = ItemStackSystem.MergeAllStacks(inventory);
```

### 7. 槽位操作

```csharp
// 获取槽位
IInventorySlot slot = inventory.GetSlot(0);

// 检查槽位状态
if (!slot.IsEmpty)
{
    IItem item = slot.Item;
    Console.WriteLine($"槽位0: {item.Name} x{item.StackCount}");
}

// 交换两个槽位
inventory.SwapSlots(0, 5);

// 拆分堆叠
ItemStackSystem.SplitStack(inventory, slotIndex: 0, splitAmount: 3);
```

### 8. Hotbar（快捷栏）使用

```csharp
using InventoryEngine.Core.Interfaces;
using InventoryEngine.Core.Implementations;

// 创建Hotbar（默认10个槽位，对应数字键1-9和0）
IHotbar hotbar = new Hotbar(10);

// 从背包添加物品到Hotbar
var inventorySlot = inventory.FindItemSlots(healthPotion.ItemConfigId)[0];
hotbar.AddFromInventory(inventory, inventorySlot, 0); // 添加到第0个槽位

// 使用Hotbar中的物品
int used = hotbar.TryUse(0, 1); // 从槽位0使用1个物品

// 刷新Hotbar（同步背包状态）
hotbar.RefreshAll(inventory);

// 交换Hotbar槽位
hotbar.SwapSlots(0, 1);

// 查看Hotbar内容
for (int i = 0; i < hotbar.Capacity; i++)
{
    var slot = hotbar.GetSlot(i);
    if (!slot.IsEmpty)
    {
        Console.WriteLine($"Hotbar槽位 {i}: {slot.Item.Name} x{slot.ItemCount}");
    }
}
```

### 9. 装备系统使用

```csharp
using InventoryEngine.Core;
using InventoryEngine.Core.Interfaces;
using InventoryEngine.Core.Implementations;

// 创建装备系统
IEquipment equipment = new Equipment();

// 装备物品（从背包）
var weaponSlot = inventory.FindItemSlots(sword.ItemConfigId)[0];
var slot = inventory.GetSlot(weaponSlot);
var oldWeapon = equipment.EquipItem(slot.Item, inventory);

if (oldWeapon == null)
{
    // 装备成功，从背包移除
    inventory.RemoveItemAt(weaponSlot, 1);
}
else
{
    // 旧装备已自动放入背包
    Console.WriteLine($"新装备: {slot.Item.Name}，旧装备: {oldWeapon.Name}");
}

// 检查物品是否可以装备
var canEquip = equipment.CanEquipItem(sword);
if (canEquip.HasValue)
{
    Console.WriteLine($"可以装备到: {canEquip.Value}");
}

// 查看已装备的物品
foreach (var item in equipment.GetEquippedItems())
{
    Console.WriteLine($"已装备: {item.Name}");
}

// 查看特定装备槽位
var weaponSlot_eq = equipment.GetSlot(EquipmentSlotType.Weapon);
if (weaponSlot_eq != null && weaponSlot_eq.IsEquipped)
{
    Console.WriteLine($"武器: {weaponSlot_eq.Item.Name}");
}

// 卸下装备
equipment.Unequip(EquipmentSlotType.Weapon, inventory); // 装备会自动放入背包
```

## 物品类型

系统支持以下物品类型：

- `Weapon` - 武器（不可堆叠）
- `Armor` - 防具（不可堆叠）
- `Consumable` - 消耗品（可堆叠）
- `Material` - 材料（可堆叠）
- `Equipment` - 装备（不可堆叠）
- `Quest` - 任务物品（可堆叠）
- `Misc` - 杂项（可堆叠）

## 物品稀有度

系统支持以下稀有度等级：

- `Common` - 普通（白色）
- `Uncommon` - uncommon（绿色）
- `Rare` - 稀有（蓝色）
- `Epic` - 史诗（紫色）
- `Legendary` - 传说（橙色）

## 装备槽位类型

系统支持以下装备槽位类型：

- `Weapon` - 主武器
- `OffHand` - 副手（盾牌/副武器）
- `Helmet` - 头盔
- `Armor` - 护甲
- `Gloves` - 手套
- `Boots` - 靴子
- `Ring` - 戒指
- `Necklace` - 项链
- `Accessory` - 饰品

每个装备槽位只能装备对应类型的物品：
- `Weapon` 和 `OffHand` 槽位：`ItemType.Weapon`
- `Helmet`、`Armor`、`Gloves`、`Boots` 槽位：`ItemType.Armor`
- `Ring`、`Necklace`、`Accessory` 槽位：`ItemType.Equipment`

## 扩展

### 自定义物品

可以继承 `InventoryItem` 类或实现 `IItem` 接口来创建自定义物品：

```csharp
public class CustomWeapon : InventoryItem
{
    public int Damage { get; private set; }
    
    public CustomWeapon(int configId, string name, int damage) 
        : base(configId, name, ItemType.Weapon)
    {
        Damage = damage;
    }
}
```

### 自定义背包

可以继承 `Inventory` 类或实现 `IInventory` 接口来创建自定义背包：

```csharp
public class LimitedInventory : Inventory
{
    private Dictionary<ItemType, int> _typeLimits;
    
    public LimitedInventory(int capacity) : base(capacity)
    {
        _typeLimits = new Dictionary<ItemType, int>();
    }
    
    public override int AddItem(IItem item, int amount = 1)
    {
        // 添加类型限制逻辑
        // ...
        return base.AddItem(item, amount);
    }
}
```

## 示例代码

完整的使用示例请参考：
- `Examples/InventoryExample.cs` - 背包基本功能示例
- `Examples/HotbarAndEquipmentExample.cs` - Hotbar和装备系统示例

## 注意事项

1. 物品的 `ItemConfigId` 用于标识相同配置的物品，相同配置的物品可以堆叠
2. 物品的 `Id` 是每个物品实例的唯一标识符
3. 不可堆叠物品的 `MaxStackSize` 应该设置为 1
4. 背包整理会按照物品类型和ID排序，并自动合并相同物品
5. 所有操作都有适当的边界检查，确保不会出现数组越界等问题
6. **Hotbar**：
   - Hotbar槽位关联背包槽位，当背包中的物品被移除或使用后，需要调用 `RefreshAll()` 同步状态
   - Hotbar中的物品使用后会自动减少堆叠数量，但需要同步到背包
7. **装备系统**：
   - 装备物品时，如果该槽位已有装备，旧装备会自动放入背包
   - 卸下装备时，装备会自动放入背包（如果背包有空间）
   - 只有符合槽位类型的物品才能装备到对应槽位

## 许可证

此背包引擎作为战争策略游戏引擎的一部分，遵循项目整体许可证。
