using System;
using System.Collections.Generic;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Examples
{
    /// <summary>
    /// 技能系统使用示例
    /// </summary>
    public class SGSKSkillExample
    {
        /// <summary>
        /// 示例：创建一个简单的主动技能
        /// </summary>
        public class ExampleActiveSkill : ISkill
        {
            public string Name => "示例主动技";
            public SkillType Type => SkillType.Active;
            public string Description => "可以主动使用的技能";

            public bool CanUse(ISGSKEngine engine, IPlayer player, List<IPlayer> targets)
            {
                // 检查技能使用条件
                return player.Hp > 0 && targets != null && targets.Count > 0;
            }

            public void Use(ISGSKEngine engine, IPlayer player, List<IPlayer> targets)
            {
                // 执行技能效果
                Console.WriteLine($"{player.Name} 使用了 {Name}");
                foreach (var target in targets)
                {
                    engine.DealDamage(player.Id, target.Id, 1, DamageType.Normal);
                }
            }

            public bool Trigger(ISGSKEngine engine, IPlayer player, string eventName, object eventData)
            {
                // 主动技能不响应触发事件
                return false;
            }
        }

        /// <summary>
        /// 示例：创建一个被动技能
        /// </summary>
        public class ExamplePassiveSkill : ISkill
        {
            public string Name => "示例被动技";
            public SkillType Type => SkillType.Passive;
            public string Description => "自动触发的被动技能";

            public bool CanUse(ISGSKEngine engine, IPlayer player, List<IPlayer> targets)
            {
                // 被动技能不能主动使用
                return false;
            }

            public void Use(ISGSKEngine engine, IPlayer player, List<IPlayer> targets)
            {
                // 被动技能不能主动使用
            }

            public bool Trigger(ISGSKEngine engine, IPlayer player, string eventName, object eventData)
            {
                // 响应特定事件
                if (eventName == "OnDamageDealt")
                {
                    var damageResult = eventData as DamageResult;
                    if (damageResult != null && damageResult.TargetId == player.Id)
                    {
                        // 受到伤害时触发，减少1点伤害
                        Console.WriteLine($"{player.Name} 的 {Name} 触发，减少1点伤害");
                        player.Hp += 1; // 恢复1点血量
                        return true;
                    }
                }
                return false;
            }
        }

        public static void RunExample()
        {
            Console.WriteLine("=== 技能系统示例 ===\n");

            // 注册技能
            var activeSkill = new ExampleActiveSkill();
            var passiveSkill = new ExamplePassiveSkill();

            SGSKEngine.Core.Systems.SkillSystem.RegisterSkill(activeSkill);
            SGSKEngine.Core.Systems.SkillSystem.RegisterSkill(passiveSkill);

            Console.WriteLine($"已注册技能: {activeSkill.Name} ({activeSkill.Type})");
            Console.WriteLine($"已注册技能: {passiveSkill.Name} ({passiveSkill.Type})");

            // 获取技能
            var skill = SGSKEngine.Core.Systems.SkillSystem.GetSkill("示例主动技");
            if (skill != null)
            {
                Console.WriteLine($"获取到技能: {skill.Name}");
                Console.WriteLine($"技能描述: {skill.Description}");
            }
        }
    }
}
