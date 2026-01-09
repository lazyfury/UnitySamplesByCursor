using System;
using PokerEngine.Core.Interfaces;
using PokerEngine.Core.Types;

namespace PokerEngine.Core.Systems
{
    /// <summary>
    /// 扑克引擎异常处理辅助类
    /// 提供安全的操作包装方法
    /// </summary>
    public static class PokerExceptionHelper
    {
        /// <summary>
        /// 安全执行玩家行动
        /// </summary>
        /// <param name="engine">引擎实例</param>
        /// <param name="playerId">玩家ID</param>
        /// <param name="actionType">行动类型</param>
        /// <param name="amount">金额（可选）</param>
        /// <param name="onError">错误回调</param>
        /// <returns>是否成功</returns>
        public static bool SafeProcessPlayerAction(
            IPokerEngine engine,
            int playerId,
            ActionType actionType,
            int amount = 0,
            Action<Exception> onError = null)
        {
            try
            {
                engine.ProcessPlayerAction(playerId, actionType, amount);
                return true;
            }
            catch (ArgumentException ex)
            {
                onError?.Invoke(ex);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                onError?.Invoke(ex);
                return false;
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
                return false;
            }
        }

        /// <summary>
        /// 安全初始化引擎
        /// </summary>
        public static bool SafeInitialize(
            IPokerEngine engine,
            System.Collections.Generic.List<IPokerPlayer> players,
            int smallBlind,
            int bigBlind,
            Action<Exception> onError = null)
        {
            try
            {
                engine.Initialize(players, smallBlind, bigBlind);
                return true;
            }
            catch (ArgumentException ex)
            {
                onError?.Invoke(ex);
                return false;
            }
        }

        /// <summary>
        /// 安全开始新局
        /// </summary>
        public static bool SafeStartNewHand(
            IPokerEngine engine,
            Action<Exception> onError = null)
        {
            try
            {
                engine.StartNewHand();
                return true;
            }
            catch (InvalidOperationException ex)
            {
                onError?.Invoke(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取异常的用户友好消息
        /// </summary>
        public static string GetUserFriendlyMessage(Exception ex)
        {
            return ex switch
            {
                ArgumentException argEx => $"参数错误: {argEx.Message}",
                InvalidOperationException opEx => $"操作错误: {opEx.Message}",
                _ => $"发生错误: {ex.Message}"
            };
        }

        /// <summary>
        /// 检查玩家是否可以执行指定行动
        /// </summary>
        public static (bool canAct, string reason) CanPlayerAct(
            IPokerEngine engine,
            IPokerPlayer player,
            ActionType actionType,
            int amount = 0)
        {
            // 检查游戏状态
            if (engine.State != GameState.Playing)
            {
                return (false, "游戏未在进行中");
            }

            // 检查是否是当前玩家
            if (engine.CurrentPlayerPosition >= engine.Players.Count ||
                engine.Players[engine.CurrentPlayerPosition].Id != player.Id)
            {
                return (false, "不是该玩家的行动时间");
            }

            // 检查玩家状态
            if (player.HasFolded)
            {
                return (false, "玩家已弃牌");
            }

            if (!player.IsActive)
            {
                return (false, "玩家未激活");
            }

            if (player.IsAllIn)
            {
                return (false, "玩家已全押");
            }

            // 检查行动类型
            int chipsToCall = engine.CurrentBet - player.CurrentBet;

            switch (actionType)
            {
                case ActionType.Check:
                    if (chipsToCall > 0)
                        return (false, "有下注时不能过牌，请选择跟注、加注或弃牌");
                    break;

                case ActionType.Call:
                    if (chipsToCall <= 0)
                        return (false, "没有需要跟注的金额，可以选择过牌");
                    if (chipsToCall > player.Chips)
                        return (false, $"筹码不足，需要 {chipsToCall}，但只有 {player.Chips}");
                    break;

                case ActionType.Bet:
                    if (engine.CurrentBet > 0)
                        return (false, "已有下注，不能下注，只能加注");
                    if (amount < engine.BigBlind)
                        return (false, $"下注金额不能小于大盲注 {engine.BigBlind}");
                    if (amount > player.Chips)
                        return (false, $"筹码不足，只有 {player.Chips}");
                    break;

                case ActionType.Raise:
                    if (engine.CurrentBet == 0)
                        return (false, "没有下注时不能加注，可以选择下注");
                    // 简化：最小加注是当前下注的2倍
                    int minRaise = engine.CurrentBet * 2;
                    if (amount < minRaise)
                        return (false, $"加注金额不能小于 {minRaise}");
                    if (amount > player.Chips + player.CurrentBet)
                        return (false, $"筹码不足");
                    break;

                case ActionType.Fold:
                case ActionType.AllIn:
                    // 这些行动总是允许的
                    break;
            }

            return (true, string.Empty);
        }
    }
}
