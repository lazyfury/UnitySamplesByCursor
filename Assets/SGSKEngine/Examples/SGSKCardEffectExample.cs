using System;
using System.Collections.Generic;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Examples
{
    /// <summary>
    /// 卡牌效果系统使用示例
    /// </summary>
    public class SGSKCardEffectExample
    {
        /// <summary>
        /// 示例：无中生有卡牌效果
        /// </summary>
        public class ExNihiloEffect : ICardEffect
        {
            public string CardName => "无中生有";
            public bool RequiresTarget => false;
            public int TargetCount => 0;

            public bool CanPlay(ISGSKEngine engine, IPlayer player, List<IPlayer> targets)
            {
                // 无中生有可以在出牌阶段使用，不需要目标
                return engine.Phase == GamePhase.Play;
            }

            public void Execute(ISGSKEngine engine, IPlayer player, List<IPlayer> targets, Card card)
            {
                // 摸两张牌
                engine.DrawCard(player.Id, 2);
                Console.WriteLine($"{player.Name} 使用了无中生有，摸了两张牌");
            }
        }

        /// <summary>
        /// 示例：过河拆桥卡牌效果
        /// </summary>
        public class DismantlementEffect : ICardEffect
        {
            public string CardName => "过河拆桥";
            public bool RequiresTarget => true;
            public int TargetCount => 1;

            public bool CanPlay(ISGSKEngine engine, IPlayer player, List<IPlayer> targets)
            {
                // 需要指定一个目标
                return engine.Phase == GamePhase.Play &&
                       targets != null &&
                       targets.Count == 1 &&
                       targets[0].Id != player.Id;
            }

            public void Execute(ISGSKEngine engine, IPlayer player, List<IPlayer> targets, Card card)
            {
                var target = targets[0];
                
                // 目标需要弃置一张牌
                if (target.Hand.Count > 0)
                {
                    // 这里简化处理，实际应该让玩家选择
                    var cardToDiscard = target.Hand[0];
                    engine.DiscardCard(target.Id, cardToDiscard.Id);
                    Console.WriteLine($"{player.Name} 对 {target.Name} 使用了过河拆桥，{target.Name} 弃置了 {cardToDiscard.Name}");
                }
            }
        }

        public static void RunExample()
        {
            Console.WriteLine("=== 卡牌效果系统示例 ===\n");

            // 注册卡牌效果
            var exNihiloEffect = new ExNihiloEffect();
            var dismantlementEffect = new DismantlementEffect();

            SGSKEngine.Core.Systems.CardEffectSystem.RegisterCardEffect(exNihiloEffect);
            SGSKEngine.Core.Systems.CardEffectSystem.RegisterCardEffect(dismantlementEffect);

            Console.WriteLine($"已注册卡牌效果: {exNihiloEffect.CardName}");
            Console.WriteLine($"已注册卡牌效果: {dismantlementEffect.CardName}");
        }
    }
}
