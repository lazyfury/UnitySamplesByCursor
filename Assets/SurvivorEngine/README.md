# SurvivorEngine - 吸血鬼幸存者类游戏核心引擎

一个简单通用的"吸血鬼幸存者"类型游戏核心引擎实现，提供自动战斗、敌人生成、升级系统等核心功能。

## 目录结构

```
SurvivorEngine/
├── Core/
│   ├── Types/              # 类型定义
│   │   ├── Vector2D.cs     # 2D向量结构
│   │   ├── GameState.cs    # 游戏状态枚举
│   │   ├── WeaponType.cs   # 武器类型枚举
│   │   ├── UpgradeType.cs  # 升级类型枚举
│   │   ├── EnemyType.cs    # 敌人类型枚举
│   │   ├── ProjectileData.cs # 投射物数据
│   │   └── UpgradeOption.cs  # 升级选项
│   ├── Interfaces/         # 接口定义
│   │   ├── IPlayer.cs      # 玩家接口
│   │   ├── IEnemy.cs       # 敌人接口
│   │   ├── IWeapon.cs      # 武器接口
│   │   └── ISurvivorEngine.cs # 游戏引擎接口
│   ├── Implementations/    # 实现类
│   │   ├── Player.cs       # 玩家实现
│   │   ├── Enemy.cs        # 敌人实现
│   │   ├── Weapon.cs       # 武器实现
│   │   └── SurvivorEngine.cs # 游戏引擎实现
│   └── Systems/            # 系统类
│       ├── EnemySpawner.cs    # 敌人生成系统
│       ├── ProjectileSystem.cs # 投射物系统
│       └── UpgradeSystem.cs   # 升级系统
└── Examples/
    └── SurvivorEngineExample.cs # 使用示例
```

## 核心功能

### 1. 玩家系统 (IPlayer)
- 生命值管理（最大生命值、当前生命值）
- 移动系统（位置、移动速度）
- 经验值和升级系统
- 拾取范围管理

### 2. 敌人系统 (IEnemy)
- 多种敌人类型（蝙蝠、骷髅、幽灵、僵尸、Boss）
- 自动寻路（追踪玩家）
- 伤害和生命值管理
- 经验值奖励

### 3. 武器系统 (IWeapon)
- 多种武器类型（鞭子、飞刀、火球、魔法棒等）
- 自动攻击系统
- 冷却时间管理
- 投射物生成和追踪
- 武器升级系统

### 4. 投射物系统 (ProjectileSystem)
- 投射物生命周期管理
- 碰撞检测
- 多种攻击模式（直线、追踪、扇形等）

### 5. 敌人生成系统 (EnemySpawner)
- 波次管理
- 动态生成间隔
- 边界生成（在玩家视野外）
- 根据波次调整难度

### 6. 升级系统 (UpgradeSystem)
- 属性升级（生命值、速度、伤害等）
- 武器升级
- 新武器解锁
- 随机升级选项生成

### 7. 游戏引擎 (SurvivorEngine)
- 游戏状态管理
- 事件系统
- 游戏循环更新
- 波次管理

## 快速开始

### 基本使用

```csharp
using SurvivorEngine.Core.Implementations;
using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

// 创建引擎实例
ISurvivorEngine engine = new SurvivorEngine();

// 初始化游戏
engine.Initialize(
    playerStartPosition: new Vector2D(0, 0),
    playerMaxHealth: 100f,
    playerMoveSpeed: 5f
);

// 订阅事件
engine.OnStateChanged += (state) => {
    Debug.Log($"游戏状态: {state}");
};

engine.OnLevelUp += (level, exp) => {
    Debug.Log($"升级到 {level} 级！");
};

engine.OnLevelUpAvailable += (upgrades) => {
    // 显示升级选项UI
    foreach (var upgrade in upgrades)
    {
        Debug.Log($"{upgrade.Name}: {upgrade.Description}");
    }
};

// 在游戏循环中更新
void Update()
{
    // 获取输入
    Vector2D input = new Vector2D(
        Input.GetAxis("Horizontal"),
        Input.GetAxis("Vertical")
    );
    
    // 设置玩家移动
    engine.SetPlayerMovement(input);
    
    // 更新引擎
    engine.Update(Time.deltaTime);
}
```

### 事件系统

引擎提供以下事件：

- `OnStateChanged`: 游戏状态改变时触发
- `OnHealthChanged`: 玩家生命值改变时触发
- `OnLevelUp`: 玩家升级时触发
- `OnEnemyKilled`: 敌人被击杀时触发
- `OnLevelUpAvailable`: 升级选项可用时触发
- `OnWaveChanged`: 新波次开始时触发

### 游戏状态

引擎支持以下游戏状态：

- `NotInitialized`: 未初始化
- `Menu`: 菜单
- `Playing`: 游戏中
- `LevelUp`: 升级选择
- `Paused`: 暂停
- `GameOver`: 游戏结束
- `Victory`: 胜利

## 武器系统

### 支持的武器类型

1. **Whip (鞭子)**: 扇形攻击，近距离高伤害
2. **Knife (飞刀)**: 追踪最近的敌人
3. **Fireball (火球)**: 直线发射，中等伤害
4. **MagicWand (魔法棒)**: 随机选择敌人攻击

### 武器属性

- `Damage`: 伤害值
- `AttackSpeed`: 攻击速度（次/秒）
- `Range`: 攻击范围
- `ProjectileSpeed`: 投射物速度
- `ProjectileCount`: 投射物数量
- `Level`: 武器等级

### 添加新武器

```csharp
engine.AddWeapon(WeaponType.Knife);
```

## 升级系统

### 属性升级

- `MaxHealth`: 增加最大生命值
- `MoveSpeed`: 增加移动速度
- `Damage`: 增加所有武器伤害
- `AttackSpeed`: 增加所有武器攻击速度
- `AttackRange`: 增加所有武器攻击范围
- `ProjectileSpeed`: 增加投射物速度
- `ExperienceGain`: 增加经验获得
- `PickupRange`: 增加拾取范围

### 武器升级

- `WeaponLevel`: 提升特定武器等级
- `NewWeapon`: 解锁新武器

### 应用升级

```csharp
// 获取升级选项
var upgrades = engine.GetAvailableUpgrades(3);

// 应用升级
engine.ApplyUpgrade(upgrades[0]);
```

## 敌人系统

### 敌人类型

- `Bat`: 蝙蝠 - 快速、低生命值
- `Skeleton`: 骷髅 - 中等速度、中等生命值
- `Ghost`: 幽灵 - 快速、低生命值
- `Zombie`: 僵尸 - 慢速、高生命值
- `Boss`: Boss - 高生命值、高伤害

### 生成敌人

```csharp
engine.SpawnEnemy(EnemyType.Bat, new Vector2D(10, 10));
```

## 扩展功能

### 自定义武器

可以在 `Weapon.cs` 的 `Attack` 方法中添加新的武器类型：

```csharp
case WeaponType.YourWeapon:
    return CreateYourWeaponProjectiles(playerPosition, enemies);
```

### 自定义敌人

可以在 `SurvivorEngine.cs` 的 `SpawnEnemy` 方法中调整敌人属性：

```csharp
case EnemyType.YourEnemy:
    maxHealth = 100f;
    moveSpeed = 2f;
    damage = 20f;
    experienceReward = 20f;
    break;
```

### 自定义升级

可以在 `UpgradeSystem.cs` 的 `GetAllPossibleUpgrades` 方法中添加新的升级选项。

## 性能优化建议

1. **对象池**: 对于频繁创建/销毁的敌人和投射物，建议使用对象池
2. **空间分区**: 对于大量敌人，可以使用空间分区优化碰撞检测
3. **批量处理**: 投射物系统已经批量处理，避免频繁的列表操作

## 注意事项

1. 引擎需要在每帧调用 `Update` 方法
2. 玩家移动方向需要在每帧通过 `SetPlayerMovement` 设置
3. 升级选择需要在 `LevelUp` 状态下通过 `ApplyUpgrade` 应用
4. 敌人会自动追踪玩家并造成伤害
5. 武器会自动攻击范围内的敌人

## 示例场景

查看 `Examples/SurvivorEngineExample.cs` 了解完整的使用示例，包括：
- 引擎初始化
- 事件订阅
- 输入处理
- 游戏循环更新
- 简单的GUI显示

## 许可证

本引擎遵循项目许可证。
