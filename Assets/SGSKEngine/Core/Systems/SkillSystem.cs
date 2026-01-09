using System.Collections.Generic;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Systems
{
    /// <summary>
    /// 技能系统
    /// </summary>
    public class SkillSystem
    {
        private static Dictionary<string, ISkill> _skills = new Dictionary<string, ISkill>();

        /// <summary>
        /// 注册技能
        /// </summary>
        public static void RegisterSkill(ISkill skill)
        {
            _skills[skill.Name] = skill;
        }

        /// <summary>
        /// 获取技能
        /// </summary>
        public static ISkill GetSkill(string skillName)
        {
            return _skills.ContainsKey(skillName) ? _skills[skillName] : null;
        }

        /// <summary>
        /// 触发技能事件
        /// </summary>
        public static void TriggerSkillEvent(ISGSKEngine engine, IPlayer player, string eventName, object eventData)
        {
            if (player?.Character?.Skills == null)
                return;

            foreach (var skillName in player.Character.Skills)
            {
                var skill = GetSkill(skillName);
                if (skill != null && skill.Type == SkillType.Passive)
                {
                    skill.Trigger(engine, player, eventName, eventData);
                }
            }
        }

        /// <summary>
        /// 检查技能是否可以使用
        /// </summary>
        public static bool CanUseSkill(ISGSKEngine engine, IPlayer player, string skillName, List<IPlayer> targets)
        {
            var skill = GetSkill(skillName);
            if (skill == null)
                return false;

            if (skill.Type == SkillType.Passive)
                return false;

            // 检查是否已使用过（限定技）
            if (skill.Type == SkillType.Limited)
            {
                if (player.SkillUsed.ContainsKey(skillName) && player.SkillUsed[skillName])
                    return false;
            }

            return skill.CanUse(engine, player, targets);
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        public static void UseSkill(ISGSKEngine engine, IPlayer player, string skillName, List<IPlayer> targets)
        {
            var skill = GetSkill(skillName);
            if (skill == null)
                return;

            if (CanUseSkill(engine, player, skillName, targets))
            {
                skill.Use(engine, player, targets);
                
                // 标记限定技已使用
                if (skill.Type == SkillType.Limited)
                {
                    player.SkillUsed[skillName] = true;
                }
            }
        }
    }
}
