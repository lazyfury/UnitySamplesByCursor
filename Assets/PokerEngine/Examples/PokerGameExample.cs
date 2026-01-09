using System;
using System.Collections.Generic;
using System.Linq;
using PokerEngine.Core.Implementations;
using PokerEngine.Core.Interfaces;
using PokerEngine.Core.Types;

namespace PokerEngine.Examples
{
    /// <summary>
    /// 德州扑克游戏使用示例
    /// </summary>
    public class PokerGameExample
    {
        public static void RunExample()
        {
            // 创建玩家
            var players = new List<IPokerPlayer>
            {
                new PokerPlayer(1, "Alice", 1000),
                new PokerPlayer(2, "Bob", 1000),
                new PokerPlayer(3, "Charlie", 1000)
            };

            // 创建引擎
            var engine = new PokerGameEngine();

            // 初始化游戏（小盲10，大盲20）
            engine.Initialize(players, 10, 20);

            // 订阅事件
            engine.OnPhaseChanged += (phase) => 
            {
                Console.WriteLine($"=== 阶段变化: {phase} ===");
            };

            engine.OnPotChanged += (pot) => 
            {
                Console.WriteLine($"底池: {pot} 筹码");
            };

            engine.OnPlayerAction += (player, action) => 
            {
                string actionStr = action.ActionType switch
                {
                    ActionType.Fold => "弃牌",
                    ActionType.Check => "过牌",
                    ActionType.Call => $"跟注 {action.Amount}",
                    ActionType.Bet => $"下注 {action.Amount}",
                    ActionType.Raise => $"加注到 {action.Amount}",
                    ActionType.AllIn => "全押",
                    _ => action.ActionType.ToString()
                };
                Console.WriteLine($"{player.Name}: {actionStr} (剩余筹码: {player.Chips})");
            };

            engine.OnHandFinished += (winners) => 
            {
                Console.WriteLine($"\n获胜者: {string.Join(", ", winners.Select(w => w.Name))}");
                foreach (var winner in winners)
                {
                    Console.WriteLine($"{winner.Name} 获得底池，当前筹码: {winner.Chips}");
                }
            };

            engine.OnPlayerEliminated += (player) => 
            {
                Console.WriteLine($"{player.Name} 被淘汰！");
            };

            // 开始新的一手牌（添加异常处理）
            Console.WriteLine("=== 开始新的一手牌 ===\n");
            try
            {
                engine.StartNewHand();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"无法开始新局: {ex.Message}");
                return;
            }

            // 显示玩家底牌（仅用于演示）
            Console.WriteLine("\n玩家底牌:");
            foreach (var player in players)
            {
                Console.WriteLine($"{player.Name}: {string.Join(" ", player.HoleCards)}");
            }

            // 模拟游戏流程
            Console.WriteLine("\n=== 翻牌前下注 ===\n");

            // 大盲后的第一个玩家行动
            var currentPlayer = players[engine.CurrentPlayerPosition];
            Console.WriteLine($"当前行动玩家: {currentPlayer.Name}");

            // 玩家可以选择跟注、加注或弃牌
            // 这里演示一个简单的游戏流程（添加异常处理）
            try
            {
                engine.ProcessPlayerAction(currentPlayer.Id, ActionType.Call);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"行动失败: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"操作失败: {ex.Message}");
            }

            // 下一个玩家
            try
            {
                currentPlayer = players[engine.CurrentPlayerPosition];
                engine.ProcessPlayerAction(currentPlayer.Id, ActionType.Call);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"玩家行动错误: {ex.Message}");
            }

            // 小盲玩家补齐盲注
            try
            {
                currentPlayer = players[engine.CurrentPlayerPosition];
                if (currentPlayer.CurrentBet < engine.CurrentBet)
                {
                    engine.ProcessPlayerAction(currentPlayer.Id, ActionType.Call);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"补齐盲注失败: {ex.Message}");
            }

            // 进入翻牌阶段
            Console.WriteLine($"\n公共牌: {string.Join(" ", engine.CommunityCards)}");

            // 继续游戏流程...
            // 在实际游戏中，这里会继续处理玩家的行动
            // 直到所有阶段完成

            Console.WriteLine("\n=== 示例结束 ===");
        }
    }
}
