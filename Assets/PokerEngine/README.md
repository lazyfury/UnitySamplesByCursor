# 德州扑克游戏核心引擎

一个完整的德州扑克（Texas Hold'em）游戏核心引擎实现，支持多人游戏、完整的下注系统、手牌评估和底池管理。

## 功能特性

- ✅ 完整的52张牌牌组管理
- ✅ 支持2-10名玩家
- ✅ 完整的游戏流程（翻牌前、翻牌、转牌、河牌、摊牌）
- ✅ 下注系统（弃牌、过牌、跟注、下注、加注、全押）
- ✅ 盲注系统（小盲、大盲）
- ✅ 手牌评估（从高牌到皇家同花顺的所有牌型）
- ✅ 底池管理（主池和边池）
- ✅ 事件系统（游戏状态变化、玩家行动、手牌结束等）

## 核心组件

### Types（类型定义）

- `Card`: 扑克牌（花色和点数）
- `Suit`: 花色枚举（黑桃、红心、方块、梅花）
- `Rank`: 点数枚举（A, 2-10, J, Q, K）
- `HandRank`: 手牌等级（高牌、一对、两对、三条、顺子、同花、葫芦、四条、同花顺、皇家同花顺）
- `GamePhase`: 游戏阶段（翻牌前、翻牌、转牌、河牌、摊牌）
- `ActionType`: 玩家行动类型（弃牌、过牌、跟注、下注、加注、全押）
- `PlayerAction`: 玩家行动数据
- `HandResult`: 手牌评估结果
- `GameState`: 游戏状态

### Interfaces（接口）

- `IPokerEngine`: 扑克引擎主接口
- `IPokerPlayer`: 玩家接口
- `IDeck`: 牌组接口

### Systems（系统）

- `HandEvaluator`: 手牌评估系统，评估玩家手牌并比较大小
- `BettingSystem`: 下注系统，管理下注逻辑和验证
- `PotManager`: 底池管理系统，处理主池和边池的分配

### Implementations（实现）

- `PokerEngine`: 扑克引擎主实现
- `PokerPlayer`: 玩家实现
- `Deck`: 牌组实现

## 使用示例

```csharp
using PokerEngine.Core.Implementations;
using PokerEngine.Core.Interfaces;
using PokerEngine.Core.Types;

// 创建玩家
var players = new List<IPokerPlayer>
{
    new PokerPlayer(1, "玩家1", 1000),
    new PokerPlayer(2, "玩家2", 1000),
    new PokerPlayer(3, "玩家3", 1000)
};

// 创建引擎
var engine = new PokerEngine();

// 初始化游戏（小盲10，大盲20）
engine.Initialize(players, 10, 20);

// 订阅事件
engine.OnPhaseChanged += (phase) => Console.WriteLine($"阶段: {phase}");
engine.OnPlayerAction += (player, action) => 
    Console.WriteLine($"{player.Name} {action.ActionType}");
engine.OnHandFinished += (winners) => 
    Console.WriteLine($"获胜者: {string.Join(", ", winners.Select(w => w.Name))}");

// 开始新的一手牌
engine.StartNewHand();

// 处理玩家行动
engine.ProcessPlayerAction(1, ActionType.Call);
engine.ProcessPlayerAction(2, ActionType.Raise, 40);
engine.ProcessPlayerAction(3, ActionType.Fold);
// ... 继续游戏流程
```

## 游戏流程

1. **初始化**: 调用 `Initialize()` 设置玩家和盲注
2. **开始新局**: 调用 `StartNewHand()` 开始新的一手牌
3. **玩家行动**: 调用 `ProcessPlayerAction()` 处理玩家行动
4. **自动推进**: 当所有玩家行动完成后，引擎自动进入下一阶段
5. **结束手牌**: 在河牌后或只剩一个玩家时，自动结束并分配底池

## 手牌评估

引擎使用 `HandEvaluator` 评估手牌，支持所有标准德州扑克牌型：

1. 皇家同花顺 (Royal Flush)
2. 同花顺 (Straight Flush)
3. 四条 (Four of a Kind)
4. 葫芦 (Full House)
5. 同花 (Flush)
6. 顺子 (Straight)
7. 三条 (Three of a Kind)
8. 两对 (Two Pair)
9. 一对 (Pair)
10. 高牌 (High Card)

## 事件系统

引擎提供以下事件：

- `OnPhaseChanged`: 游戏阶段变化
- `OnPotChanged`: 底池金额变化
- `OnPlayerAction`: 玩家执行行动
- `OnHandFinished`: 一手牌结束，返回获胜者列表
- `OnPlayerEliminated`: 玩家被淘汰（筹码为0）

## 异常处理

引擎会在以下情况抛出异常：

- `ArgumentException`: 参数错误（如玩家数量不足、无效的行动等）
- `InvalidOperationException`: 操作错误（如游戏未初始化、不是当前玩家行动时间等）

### 使用异常处理辅助类

```csharp
using PokerEngine.Core.Systems;

// 安全执行玩家行动
bool success = PokerExceptionHelper.SafeProcessPlayerAction(
    engine, 
    playerId, 
    ActionType.Call,
    onError: (ex) => Console.WriteLine($"错误: {ex.Message}")
);

// 检查玩家是否可以执行行动
var (canAct, reason) = PokerExceptionHelper.CanPlayerAct(
    engine, 
    player, 
    ActionType.Raise, 
    100
);
if (!canAct)
{
    Console.WriteLine($"无法执行: {reason}");
}
```

### 手动异常处理

```csharp
try
{
    engine.ProcessPlayerAction(playerId, ActionType.Call);
}
catch (ArgumentException ex)
{
    // 处理参数错误（如无效的行动）
    Console.WriteLine($"参数错误: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    // 处理操作错误（如游戏状态不正确）
    Console.WriteLine($"操作错误: {ex.Message}");
}
```

详细示例请参考 `PokerExceptionHandlingExample.cs`。

## 注意事项

- 引擎支持2-10名玩家
- 自动处理边池（当有玩家全押时）
- 庄家位置每局自动移动
- 支持玩家中途加入和退出
- 所有下注验证由引擎自动处理
- **建议使用异常处理辅助类或 try-catch 包装所有引擎操作**