# 三国杀风格卡牌战斗游戏核心引擎

一个完整的类似三国杀的卡牌战斗游戏核心引擎实现，支持角色系统、卡牌系统、回合系统、技能系统和战斗系统。

## 功能特性

- ✅ 完整的卡牌系统（基本牌、锦囊牌、装备牌）
- ✅ 角色系统（血量、技能、装备）
- ✅ 回合系统（准备、判定、摸牌、出牌、弃牌、结束）
- ✅ 技能系统（主动、被动、限定、觉醒）
- ✅ 战斗系统（攻击、闪避、伤害计算）
- ✅ 距离系统（座位距离、装备修正）
- ✅ 判定系统（延时锦囊判定）
- ✅ 事件系统（回合开始/结束、卡牌使用、伤害等）

## 核心组件

### Types（类型定义）

- `CardType`: 卡牌类型（基本牌、锦囊牌、装备牌）
- `CardSuit`: 卡牌花色（黑桃、红桃、方块、梅花）
- `CardRank`: 卡牌点数（1-13）
- `Card`: 卡牌实体
- `Character`: 角色数据
- `GamePhase`: 游戏阶段（准备、判定、摸牌、出牌、弃牌、结束）
- `GameState`: 游戏状态（未初始化、游戏中、已结束）
- `PlayerState`: 玩家状态（存活、濒死、死亡）
- `DamageType`: 伤害类型（普通、火焰、雷电）
- `SkillType`: 技能类型（主动、被动、限定、觉醒）
- `EquipmentSlot`: 装备槽位（武器、防具、进攻马、防御马）
- `DamageResult`: 伤害结果

### Interfaces（接口）

- `ISGSKEngine`: 游戏引擎主接口
- `IPlayer`: 玩家接口
- `IDeck`: 牌堆接口
- `ISkill`: 技能接口
- `ICardEffect`: 卡牌效果接口

### Systems（系统）

- `CombatSystem`: 战斗系统，处理攻击、决斗等战斗逻辑
- `DistanceSystem`: 距离系统，计算玩家间距离和攻击范围
- `JudgmentSystem`: 判定系统，处理延时锦囊的判定
- `CardEffectSystem`: 卡牌效果系统，管理卡牌效果的注册和执行
- `SkillSystem`: 技能系统，管理技能的注册、使用和触发

### Implementations（实现）

- `SGSKEngine`: 游戏引擎主实现
- `Player`: 玩家实现
- `Deck`: 牌堆实现

## 使用示例

### 基本游戏流程

```csharp
using SGSKEngine.Core.Implementations;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

// 1. 创建角色
var character1 = new Character(1, "刘备", 4, "主公", "男");
character1.Skills.Add("仁德");

var character2 = new Character(2, "关羽", 4, "忠臣", "男");
character2.Skills.Add("武圣");

// 2. 创建玩家
var players = new List<IPlayer>
{
    new Player(1, "玩家1", character1),
    new Player(2, "玩家2", character2)
};

// 3. 创建引擎
var engine = new SGSKEngine();

// 4. 订阅事件
engine.OnTurnStarted += (playerId) => 
{
    var player = engine.GetPlayer(playerId);
    Console.WriteLine($"{player.Name} 的回合开始");
};

engine.OnDamageDealt += (result) => 
{
    if (result.Dealt)
    {
        var target = engine.GetPlayer(result.TargetId);
        Console.WriteLine($"{target.Name} 受到 {result.Amount} 点伤害");
    }
};

// 5. 初始化并开始游戏
engine.Initialize(players);
engine.StartGame();

// 6. 游戏流程
// 引擎会自动处理回合切换和阶段推进
// 玩家可以在出牌阶段使用卡牌
var currentPlayer = engine.GetPlayer(engine.CurrentPlayerId);
var hand = engine.GetPlayerHand(currentPlayer.Id);

// 出杀
var slashCard = hand.Find(c => c.Name.Contains("杀"));
if (slashCard != null)
{
    var target = engine.AlivePlayers.Find(p => p.Id != currentPlayer.Id);
    if (target != null && engine.IsInRange(currentPlayer.Id, target.Id))
    {
        engine.PlayCard(currentPlayer.Id, slashCard.Id, target.Id);
    }
}

// 进入下一阶段
engine.NextPhase();
```

### 技能系统

```csharp
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

// 创建技能
public class MySkill : ISkill
{
    public string Name => "我的技能";
    public SkillType Type => SkillType.Active;
    public string Description => "技能描述";

    public bool CanUse(ISGSKEngine engine, IPlayer player, List<IPlayer> targets)
    {
        // 检查使用条件
        return player.Hp > 0;
    }

    public void Use(ISGSKEngine engine, IPlayer player, List<IPlayer> targets)
    {
        // 执行技能效果
        foreach (var target in targets)
        {
            engine.DealDamage(player.Id, target.Id, 1);
        }
    }

    public bool Trigger(ISGSKEngine engine, IPlayer player, string eventName, object eventData)
    {
        // 被动技能触发逻辑
        return false;
    }
}

// 注册技能
var skill = new MySkill();
SkillSystem.RegisterSkill(skill);

// 使用技能
var player = engine.GetPlayer(playerId);
var targets = new List<IPlayer> { engine.GetPlayer(targetId) };
SkillSystem.UseSkill(engine, player, "我的技能", targets);
```

### 卡牌效果系统

```csharp
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

// 创建卡牌效果
public class MyCardEffect : ICardEffect
{
    public string CardName => "我的卡牌";
    public bool RequiresTarget => true;
    public int TargetCount => 1;

    public bool CanPlay(ISGSKEngine engine, IPlayer player, List<IPlayer> targets)
    {
        return engine.Phase == GamePhase.Play && 
               targets != null && 
               targets.Count == 1;
    }

    public void Execute(ISGSKEngine engine, IPlayer player, List<IPlayer> targets, Card card)
    {
        // 执行卡牌效果
        var target = targets[0];
        engine.DealDamage(player.Id, target.Id, 2);
    }
}

// 注册卡牌效果
var effect = new MyCardEffect();
CardEffectSystem.RegisterCardEffect(effect);
```

## 游戏流程

1. **初始化**: 调用 `Initialize()` 设置玩家
2. **开始游戏**: 调用 `StartGame()` 开始游戏
3. **回合流程**:
   - 准备阶段：重置状态
   - 判定阶段：处理延时锦囊
   - 摸牌阶段：摸2张牌
   - 出牌阶段：玩家可以出牌、使用技能
   - 弃牌阶段：弃置超过血量的手牌
   - 结束阶段：回合结束，切换到下一个玩家
4. **游戏结束**: 当只剩一个势力或满足胜利条件时结束

## 卡牌类型

### 基本牌
- **杀**: 对目标造成1点伤害
- **闪**: 抵消一次杀的攻击
- **桃**: 恢复1点血量，或在濒死时使用

### 锦囊牌
- **过河拆桥**: 弃置目标一张牌
- **顺手牵羊**: 获得目标一张牌
- **无中生有**: 摸两张牌
- **决斗**: 双方轮流出杀
- **万箭齐发**: 所有玩家需要出闪
- **南蛮入侵**: 所有玩家需要出杀
- **桃园结义**: 所有玩家恢复1点血量
- **五谷丰登**: 所有玩家摸牌
- **无懈可击**: 抵消锦囊牌效果
- **乐不思蜀**（延时）: 判定阶段判定，非红桃则跳过出牌阶段
- **闪电**（延时）: 判定阶段判定，黑桃2-9造成3点雷电伤害
- **兵粮寸断**（延时）: 判定阶段判定，非梅花则跳过摸牌阶段

### 装备牌
- **武器**: 增加攻击范围
- **防具**: 提供防御效果
- **进攻马（+1马）**: 减少攻击距离
- **防御马（-1马）**: 增加防御距离

## 事件系统

引擎提供以下事件：

- `OnPhaseChanged`: 游戏阶段变化
- `OnTurnStarted`: 玩家回合开始
- `OnTurnEnded`: 玩家回合结束
- `OnCardPlayed`: 卡牌被打出
- `OnCardUsed`: 卡牌被使用
- `OnDamageDealt`: 造成伤害
- `OnPlayerDied`: 玩家死亡
- `OnPlayerStateChanged`: 玩家状态变化

## 距离系统

- 默认攻击范围为1
- 座位距离：顺时针和逆时针的最小值
- 武器可以增加攻击范围
- 进攻马（+1马）减少攻击距离
- 防御马（-1马）增加防御距离

## 判定系统

- 从牌堆顶抽取一张牌作为判定牌
- 根据判定牌的花色和点数决定效果
- 判定牌进入弃牌堆
- 支持延时锦囊的判定

## 注意事项

- 引擎支持2-8名玩家
- 回合和阶段由引擎自动管理
- 玩家需要在合适的阶段使用卡牌和技能
- 距离计算考虑座位位置和装备
- 手牌数不能超过当前血量（弃牌阶段）
- 建议使用事件系统实现游戏逻辑
- 技能和卡牌效果需要注册后才能使用

## 扩展建议

1. **技能实现**: 实现更多角色的技能
2. **卡牌效果**: 实现更多锦囊牌的效果
3. **装备效果**: 实现装备的特殊效果
4. **AI系统**: 实现AI玩家
5. **网络支持**: 添加多人网络对战支持
6. **UI系统**: 创建游戏界面

详细示例请参考 `Examples` 目录下的示例文件。
