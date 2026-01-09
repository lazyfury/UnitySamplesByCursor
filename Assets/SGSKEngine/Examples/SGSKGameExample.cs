using System;
using System.Collections.Generic;
using SGSKEngine.Core.Implementations;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Examples
{
    /// <summary>
    /// 三国杀风格卡牌游戏示例
    /// </summary>
    public class SGSKGameExample
    {
        public static void RunExample()
        {
            Console.WriteLine("=== 三国杀风格卡牌游戏引擎示例 ===\n");

            // 1. 创建角色
            var character1 = new Character(1, "刘备", 4, "主公", "男");
            character1.Skills.Add("仁德");

            var character2 = new Character(2, "关羽", 4, "忠臣", "男");
            character2.Skills.Add("武圣");

            var character3 = new Character(3, "张飞", 4, "反贼", "男");
            character3.Skills.Add("咆哮");

            // 2. 创建玩家
            var players = new List<IPlayer>
            {
                new Player(1, "玩家1", character1),
                new Player(2, "玩家2", character2),
                new Player(3, "玩家3", character3)
            };

            // 3. 创建引擎
            var engine = new SGSKGameEngine();

            // 4. 订阅事件
            engine.OnPhaseChanged += (phase) => 
                Console.WriteLine($"阶段变化: {phase}");
            
            engine.OnTurnStarted += (playerId) => 
            {
                var player = engine.GetPlayer(playerId);
                Console.WriteLine($"\n=== {player.Name} 的回合开始 ===");
            };

            engine.OnTurnEnded += (playerId) => 
            {
                var player = engine.GetPlayer(playerId);
                Console.WriteLine($"=== {player.Name} 的回合结束 ===\n");
            };

            engine.OnCardPlayed += (player, card) => 
                Console.WriteLine($"{player.Name} 打出了 {card.Name}");

            engine.OnDamageDealt += (result) => 
            {
                if (result.Dealt)
                {
                    var source = engine.GetPlayer(result.SourceId);
                    var target = engine.GetPlayer(result.TargetId);
                    Console.WriteLine($"{source?.Name ?? "系统"} 对 {target?.Name} 造成了 {result.Amount} 点伤害");
                    Console.WriteLine($"{target?.Name} 当前血量: {target?.Hp}/{target?.MaxHp}");
                }
            };

            engine.OnPlayerDied += (sourceId, targetId) => 
            {
                var target = engine.GetPlayer(targetId);
                Console.WriteLine($"*** {target?.Name} 死亡！ ***");
            };

            // 5. 初始化游戏
            engine.Initialize(players);
            Console.WriteLine("游戏初始化完成");
            Console.WriteLine($"玩家数量: {engine.Players.Count}");
            Console.WriteLine($"牌堆剩余: {engine.Deck.Count} 张\n");

            // 6. 开始游戏
            engine.StartGame();

            // 7. 模拟游戏流程
            var currentPlayer = engine.GetPlayer(engine.CurrentPlayerId);
            Console.WriteLine($"当前玩家: {currentPlayer.Name}");
            Console.WriteLine($"当前阶段: {engine.Phase}");

            // 摸牌阶段（自动）
            Console.WriteLine("\n进入摸牌阶段...");
            engine.NextPhase(); // 进入出牌阶段

            // 出牌阶段
            Console.WriteLine($"\n进入出牌阶段，{currentPlayer.Name} 可以出牌");
            var hand = engine.GetPlayerHand(currentPlayer.Id);
            Console.WriteLine($"{currentPlayer.Name} 手牌数: {hand.Count}");

            // 尝试出杀（如果有的话）
            var slashCard = hand.Find(c => c.Name.Contains("杀"));
            if (slashCard != null)
            {
                var targets = engine.AlivePlayers.FindAll(p => p.Id != currentPlayer.Id);
                if (targets.Count > 0)
                {
                    var target = targets[0];
                    Console.WriteLine($"尝试对 {target.Name} 使用杀");
                    
                    if (engine.IsInRange(currentPlayer.Id, target.Id))
                    {
                        engine.PlayCard(currentPlayer.Id, slashCard.Id, target.Id);
                    }
                    else
                    {
                        Console.WriteLine($"距离不足，无法攻击 {target.Name}");
                    }
                }
            }

            // 结束出牌阶段
            engine.NextPhase(); // 进入弃牌阶段
            engine.NextPhase(); // 进入结束阶段
            engine.NextPhase(); // 结束回合，切换到下一个玩家

            // 继续几轮
            for (int i = 0; i < 3 && engine.State == GameState.Playing; i++)
            {
                currentPlayer = engine.GetPlayer(engine.CurrentPlayerId);
                Console.WriteLine($"\n=== 第 {i + 2} 轮 ===");
                Console.WriteLine($"当前玩家: {currentPlayer.Name}");

                // 快速过回合
                while (engine.Phase != GamePhase.Finish && engine.State == GameState.Playing)
                {
                    engine.NextPhase();
                }
                engine.NextPhase(); // 结束回合
            }

            Console.WriteLine("\n=== 示例结束 ===");
        }
    }
}
