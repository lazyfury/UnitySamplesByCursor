using System;
using System.Collections.Generic;
using System.Linq;
using PokerEngine.Core.Implementations;
using PokerEngine.Core.Interfaces;
using PokerEngine.Core.Types;

namespace PokerEngine.Examples
{
    /// <summary>
    /// 异常处理示例
    /// 展示如何捕获和处理引擎中的各种异常
    /// </summary>
    public class PokerExceptionHandlingExample
    {
        public static void RunExample()
        {
            var engine = new PokerGameEngine();

            // ========== 1. 初始化异常处理 ==========
            try
            {
                // 错误：玩家数量不足
                var invalidPlayers = new List<IPokerPlayer>
                {
                    new PokerPlayer(1, "Player1", 1000)
                };
                engine.Initialize(invalidPlayers, 10, 20);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"初始化错误: {ex.Message}");
                // 处理：提示需要至少2名玩家
            }

            // ========== 2. 游戏操作异常处理 ==========
            var players = new List<IPokerPlayer>
            {
                new PokerPlayer(1, "Alice", 1000),
                new PokerPlayer(2, "Bob", 1000)
            };

            try
            {
                engine.Initialize(players, 10, 20);
                engine.StartNewHand();

                // 错误：在游戏未开始时尝试处理行动
                // engine.ProcessPlayerAction(1, ActionType.Call);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"游戏操作错误: {ex.Message}");
                // 处理：检查游戏状态
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"参数错误: {ex.Message}");
                // 处理：验证玩家ID或行动参数
            }

            // ========== 3. 玩家行动异常处理 ==========
            try
            {
                engine.StartNewHand();
                var currentPlayer = players[engine.CurrentPlayerPosition];

                // 错误：尝试无效的行动（例如：在没有下注时加注）
                engine.ProcessPlayerAction(currentPlayer.Id, ActionType.Raise, 100);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"玩家行动错误: {ex.Message}");
                // 处理：提示玩家选择有效的行动
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"操作错误: {ex.Message}");
            }

            // ========== 4. 安全的游戏流程包装方法 ==========
            SafeGameFlow(engine, players);
        }

        /// <summary>
        /// 安全的游戏流程，包含完整的异常处理
        /// </summary>
        private static void SafeGameFlow(PokerGameEngine engine, List<IPokerPlayer> players)
        {
            try
            {
                // 初始化
                if (players.Count < 2)
                {
                    Console.WriteLine("错误：至少需要2名玩家");
                    return;
                }

                engine.Initialize(players, 10, 20);
                engine.StartNewHand();

                // 处理玩家行动
                while (engine.Phase != GamePhase.Finished && engine.Phase != GamePhase.Showdown)
                {
                    var currentPlayer = players[engine.CurrentPlayerPosition];
                    
                    try
                    {
                        // 根据游戏状态决定行动
                        if (engine.CurrentBet == 0)
                        {
                            // 可以过牌或下注
                            engine.ProcessPlayerAction(currentPlayer.Id, ActionType.Check);
                        }
                        else
                        {
                            // 必须跟注、加注或弃牌
                            int chipsToCall = engine.CurrentBet - currentPlayer.CurrentBet;
                            if (chipsToCall <= currentPlayer.Chips)
                            {
                                engine.ProcessPlayerAction(currentPlayer.Id, ActionType.Call);
                            }
                            else
                            {
                                engine.ProcessPlayerAction(currentPlayer.Id, ActionType.Fold);
                            }
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"玩家 {currentPlayer.Name} 行动失败: {ex.Message}");
                        // 自动弃牌作为后备方案
                        try
                        {
                            engine.ProcessPlayerAction(currentPlayer.Id, ActionType.Fold);
                        }
                        catch
                        {
                            // 如果连弃牌都失败，可能需要强制结束游戏
                            Console.WriteLine($"无法处理玩家 {currentPlayer.Name} 的行动，跳过");
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"操作失败: {ex.Message}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"游戏流程错误: {ex.GetType().Name} - {ex.Message}");
                Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// 验证玩家行动是否有效（在调用引擎之前）
        /// </summary>
        public static bool ValidatePlayerAction(
            PokerGameEngine engine, 
            IPokerPlayer player, 
            ActionType actionType, 
            int amount)
        {
            // 检查游戏状态
            if (engine.State != GameState.Playing)
            {
                Console.WriteLine("游戏未在进行中");
                return false;
            }

            // 检查是否是当前玩家
            if (engine.CurrentPlayerPosition >= engine.Players.Count ||
                engine.Players[engine.CurrentPlayerPosition].Id != player.Id)
            {
                Console.WriteLine("不是该玩家的行动时间");
                return false;
            }

            // 检查玩家状态
            if (player.HasFolded || !player.IsActive)
            {
                Console.WriteLine("玩家已弃牌或未激活");
                return false;
            }

            // 检查行动类型和金额
            int chipsToCall = engine.CurrentBet - player.CurrentBet;
            
            switch (actionType)
            {
                case ActionType.Check:
                    if (chipsToCall > 0)
                    {
                        Console.WriteLine("有下注时不能过牌");
                        return false;
                    }
                    break;

                case ActionType.Call:
                    if (chipsToCall <= 0)
                    {
                        Console.WriteLine("没有需要跟注的金额");
                        return false;
                    }
                    if (chipsToCall > player.Chips)
                    {
                        Console.WriteLine("筹码不足，无法跟注");
                        return false;
                    }
                    break;

                case ActionType.Bet:
                    if (engine.CurrentBet > 0)
                    {
                        Console.WriteLine("已有下注，不能下注，只能加注");
                        return false;
                    }
                    if (amount < engine.BigBlind)
                    {
                        Console.WriteLine($"下注金额不能小于大盲注 {engine.BigBlind}");
                        return false;
                    }
                    if (amount > player.Chips)
                    {
                        Console.WriteLine("筹码不足");
                        return false;
                    }
                    break;

                case ActionType.Raise:
                    if (engine.CurrentBet == 0)
                    {
                        Console.WriteLine("没有下注时不能加注");
                        return false;
                    }
                    int minRaise = engine.CurrentBet + (engine.CurrentBet - (engine.Players
                        .FirstOrDefault(p => p.CurrentBet == engine.CurrentBet)?.CurrentBet ?? 0));
                    if (amount < minRaise)
                    {
                        Console.WriteLine($"加注金额不能小于 {minRaise}");
                        return false;
                    }
                    if (amount > player.Chips + player.CurrentBet)
                    {
                        Console.WriteLine("筹码不足");
                        return false;
                    }
                    break;
            }

            return true;
        }
    }
}
