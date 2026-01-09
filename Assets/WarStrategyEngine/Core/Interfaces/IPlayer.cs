using System.Collections.Generic;
using WarStrategyEngine.Core.Systems;


namespace WarStrategyEngine.Core.Interfaces
{
    /// <summary>
    /// 玩家接口
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// 玩家ID
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// 玩家名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 所属阵营
        /// </summary>
        Faction Faction { get; }
        
        /// <summary>
        /// 玩家资源
        /// </summary>
        ResourceManager Resources { get; }
        
        /// <summary>
        /// 玩家拥有的单位
        /// </summary>
        IEnumerable<IUnit> Units { get; }
        
        /// <summary>
        /// 是否是人类玩家
        /// </summary>
        bool IsHuman { get; }
        
        /// <summary>
        /// 是否失败（失去所有单位）
        /// </summary>
        bool IsDefeated { get; }
    }
}
