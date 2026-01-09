using System.Collections.Generic;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Interfaces
{
    /// <summary>
    /// 卡牌效果接口
    /// </summary>
    public interface ICardEffect
    {
        /// <summary>
        /// 卡牌名称
        /// </summary>
        string CardName { get; }
        
        /// <summary>
        /// 是否可以打出
        /// </summary>
        bool CanPlay(ISGSKEngine engine, IPlayer player, List<IPlayer> targets);
        
        /// <summary>
        /// 执行卡牌效果
        /// </summary>
        void Execute(ISGSKEngine engine, IPlayer player, List<IPlayer> targets, Card card);
        
        /// <summary>
        /// 是否需要目标
        /// </summary>
        bool RequiresTarget { get; }
        
        /// <summary>
        /// 目标数量
        /// </summary>
        int TargetCount { get; }
    }
}
