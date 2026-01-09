# Match3Engine - 三消游戏核心引擎

一个完整的三消游戏核心引擎实现，提供游戏板管理、匹配检测、消除、下落等核心功能。

## 目录结构

```
Match3Engine/
├── Core/
│   ├── Types/              # 类型定义
│   │   ├── GemType.cs      # 宝石类型枚举
│   │   ├── GameState.cs    # 游戏状态枚举
│   │   ├── BoardPosition.cs # 游戏板位置结构
│   │   ├── MatchResult.cs  # 匹配结果
│   │   └── SwapResult.cs   # 交换结果
│   ├── Interfaces/         # 接口定义
│   │   ├── IGem.cs         # 宝石接口
│   │   ├── IGameBoard.cs   # 游戏板接口
│   │   └── IMatch3Engine.cs # 游戏引擎接口
│   ├── Implementations/    # 实现类
│   │   ├── Gem.cs          # 宝石实现
│   │   ├── GameBoard.cs    # 游戏板实现
│   │   └── Match3Engine.cs # 游戏引擎实现
│   └── Systems/            # 系统类
│       ├── MatchDetector.cs    # 匹配检测系统
│       ├── MatchEliminator.cs  # 消除系统
│       ├── FallSystem.cs       # 下落系统
│       └── SwapSystem.cs       # 交换系统
└── Examples/
    └── Match3EngineExample.cs  # 使用示例
```

## 核心功能

### 1. 游戏板管理 (IGameBoard)
- 管理游戏板网格和宝石位置
- 提供宝石的增删改查操作
- 自动生成随机宝石，避免初始匹配
- 支持交换、移除、清空等操作

### 2. 匹配检测系统 (MatchDetector)
- 检测水平匹配（3个或更多相同宝石）
- 检测垂直匹配（3个或更多相同宝石）
- 支持L型和T型匹配的合并
- 可验证交换是否会产生匹配

### 3. 消除系统 (MatchEliminator)
- 消除所有匹配的宝石
- 计算匹配得分（支持连击倍数）
- 基础分数：匹配长度 × 10
- 额外奖励：超过3个的每个宝石 +10分

### 4. 下落系统 (FallSystem)
- 自动让宝石下落填充空位
- 从顶部填充新的随机宝石
- 支持连续下落处理

### 5. 交换系统 (SwapSystem)
- 验证交换位置是否相邻
- 验证交换是否会产生匹配
- 自动撤销无效交换

### 6. 游戏引擎 (Match3Engine)
- 协调所有系统的工作
- 管理游戏状态机
- 支持连击系统（最大5倍）
- 提供事件系统（匹配、消除、分数变化等）
- 支持移动次数限制
- 自动处理匹配链（连击）

## 快速开始

### 基本使用

```csharp
using Match3Engine.Core.Implementations;
using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Types;

// 创建引擎实例
IMatch3Engine engine = new Match3Engine();

// 初始化8x8的游戏板
engine.Initialize(8, 8);

// 设置移动次数限制（可选）
engine.RemainingMoves = 30;

// 订阅事件
engine.OnMatchFound += (match) => {
    Debug.Log($"匹配到 {match.MatchLength} 个 {match.GemType} 宝石");
};

engine.OnScoreChanged += (score) => {
    Debug.Log($"当前分数: {score}");
};

// 尝试交换两个位置的宝石
BoardPosition pos1 = new BoardPosition(3, 4);
BoardPosition pos2 = new BoardPosition(3, 5);
SwapResult result = engine.TrySwap(pos1, pos2);

if (result.Success)
{
    Debug.Log("交换成功！");
}
else
{
    Debug.Log($"交换失败: {result.Reason}");
}

// 在游戏循环中更新
void Update()
{
    engine.Update(Time.deltaTime);
}
```

### 事件系统

引擎提供以下事件：

- `OnMatchFound`: 当检测到匹配时触发，参数为 `MatchResult`
- `OnEliminationComplete`: 当消除完成时触发
- `OnFallComplete`: 当下落完成时触发
- `OnScoreChanged`: 当分数变化时触发，参数为新分数
- `OnGameOver`: 当游戏结束时触发

### 游戏状态

引擎支持以下游戏状态：

- `NotInitialized`: 未初始化
- `Ready`: 准备就绪，等待输入
- `WaitingForInput`: 等待玩家输入
- `Swapping`: 交换中
- `Checking`: 检查匹配
- `Eliminating`: 消除中
- `Falling`: 下落中
- `Filling`: 填充中
- `Paused`: 暂停
- `GameOver`: 游戏结束

## 扩展功能

### 自定义宝石类型

可以通过修改 `GemType` 枚举来添加新的宝石类型：

```csharp
public enum GemType
{
    None = 0,
    Red,
    Blue,
    Green,
    Yellow,
    Purple,
    Orange,
    Special,
    Bomb,        // 新增：炸弹宝石
    Lightning    // 新增：闪电宝石
}
```

### 特殊宝石效果

可以在 `MatchEliminator` 中添加特殊宝石的处理逻辑：

```csharp
// 在消除时检查特殊宝石
if (gem.IsSpecial)
{
    // 执行特殊效果（如消除整行/整列）
}
```

### 移动次数限制

```csharp
engine.RemainingMoves = 30; // 设置30次移动限制

// 当移动次数用完时，游戏会自动结束
if (engine.RemainingMoves <= 0)
{
    // 游戏结束
}
```

## 性能优化建议

1. **批量处理匹配**: 引擎会自动合并重叠的匹配，减少重复处理
2. **延迟状态转换**: 通过 `STATE_CHANGE_DELAY` 控制状态转换延迟，避免过于频繁的状态切换
3. **对象池**: 对于频繁创建/销毁的宝石，建议使用对象池

## 注意事项

1. 游戏板大小至少为 3x3
2. 交换操作要求两个位置必须相邻（上下左右）
3. 交换必须产生至少一个匹配才会成功
4. 引擎会自动处理匹配链（连击），无需手动调用
5. 初始化的游戏板会自动确保没有初始匹配

## 示例场景

查看 `Examples/Match3EngineExample.cs` 了解完整的使用示例，包括：
- 引擎初始化
- 事件订阅
- 输入处理
- 游戏循环更新
- 调试绘制

## 许可证

本引擎遵循项目许可证。
