using System;
using System.Collections.Generic;
using PokerEngine.Core.Implementations;
using PokerEngine.Core.Interfaces;
using PokerEngine.Core.Systems;
using PokerEngine.Core.Types;

namespace PokerEngine.Examples
{
    /// <summary>
    /// 安全使用示例 - 展示如何使用异常处理辅助类
    /// </summary>
    public class PokerSafeUsageExample
    {
        public static void RunExample()
        {
            var players = new List<IPokerPlayer>
            {
                new PokerPlayer(1, "Alice", 1000),
                new PokerPlayer(2, "Bob", 1000)
            };

            var engine = new PokerGameEngine();

            // ========== 方法1: 使用辅助类进行安全操作 ==========
            Console.WriteLine("=== 方法1: 使用辅助类 ===");

            // 安全初始化
            if (!PokerExceptionHelper.SafeInitialize(engine, players, 10, 20, 
                ex => Console.WriteLine($"初始化失败: {PokerExceptionHelper.GetUserFriendlyMessage(ex)}")))
            {
                return; // 初始化失败，退出
            }

            // 安全开始新局
            if (!PokerExceptionHelper.SafeStartNewHand(engine,
                ex => Console.WriteLine($"开始新局失败: {PokerExceptionHelper.GetUserFriendlyMessage(ex)}")))
            {
                return;
            }

            // 安全处理玩家行动
            var currentPlayer = players[engine.CurrentPlayerPosition];
            bool success = PokerExceptionHelper.SafeProcessPlayerAction(
                engine,
                currentPlayer.Id,
                ActionType.Call,
                onError: ex => Console.WriteLine($"行动失败: {PokerExceptionHelper.GetUserFriendlyMessage(ex)}")
            );

            if (success)
            {
                Console.WriteLine($"{currentPlayer.Name} 成功跟注");
            }

            // ========== 方法2: 先检查再执行 ==========
            Console.WriteLine("\n=== 方法2: 先检查再执行 ===");

            currentPlayer = players[engine.CurrentPlayerPosition];
            var (canAct, reason) = PokerExceptionHelper.CanPlayerAct(
                engine,
                currentPlayer,
                ActionType.Raise,
                50
            );

            if (canAct)
            {
                // 安全执行
                PokerExceptionHelper.SafeProcessPlayerAction(
                    engine,
                    currentPlayer.Id,
                    ActionType.Raise,
                    50
                );
                Console.WriteLine($"{currentPlayer.Name} 成功加注到 50");
            }
            else
            {
                Console.WriteLine($"{currentPlayer.Name} 无法加注: {reason}");
                // 提供替代选项
                Console.WriteLine("建议: 选择跟注或弃牌");
            }

            // ========== 方法3: 传统 try-catch ==========
            Console.WriteLine("\n=== 方法3: 传统 try-catch ===");

            try
            {
                currentPlayer = players[engine.CurrentPlayerPosition];
                engine.ProcessPlayerAction(currentPlayer.Id, ActionType.Check);
                Console.WriteLine($"{currentPlayer.Name} 成功过牌");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"参数错误: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"操作错误: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"未知错误: {ex.Message}");
            }
        }
    }
}
