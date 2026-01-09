using System.Collections.Generic;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Interfaces
{
    /// <summary>
    /// 技能接口
    /// </summary>
    public interface ISkill
    {
        /// <summary>
        /// 技能名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 技能类型
        /// </summary>
        SkillType Type { get; }
        
        /// <summary>
        /// 技能描述
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// 是否可以使用
        /// </summary>
        bool CanUse(ISGSKEngine engine, IPlayer player, List<IPlayer> targets);
        
        /// <summary>
        /// 使用技能
        /// </summary>
        void Use(ISGSKEngine engine, IPlayer player, List<IPlayer> targets);
        
        /// <summary>
        /// 触发技能（被动技能）
        /// </summary>
        bool Trigger(ISGSKEngine engine, IPlayer player, string eventName, object eventData);
    }
}
