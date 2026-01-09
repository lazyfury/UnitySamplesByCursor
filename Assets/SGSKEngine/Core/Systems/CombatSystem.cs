using System.Collections.Generic;
using System.Linq;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Systems
{
    /// <summary>
    /// 战斗系统
    /// </summary>
    public class CombatSystem
    {
        /// <summary>
        /// 处理攻击
        /// </summary>
        public static bool ProcessAttack(ISGSKEngine engine, int sourceId, int targetId, Card slashCard = null)
        {
            var source = engine.GetPlayer(sourceId);
            var target = engine.GetPlayer(targetId);

            if (source == null || target == null)
                return false;

            if (source.State != PlayerState.Alive || target.State != PlayerState.Alive)
                return false;

            // 检查距离
            if (!engine.IsInRange(sourceId, targetId))
                return false;

            // 检查是否已使用过杀
            if (source.HasUsedSlash && slashCard == null)
                return false;

            // 如果使用杀牌，检查是否有杀
            if (slashCard != null)
            {
                if (!source.HasCard(slashCard.Id))
                    return false;

                if (slashCard.Type != CardType.Basic || 
                    !slashCard.Name.Contains("杀"))
                    return false;
            }

            // 目标可以出闪
            var dodgeCards = target.Hand.Where(c => 
                c.Type == CardType.Basic && 
                c.Name.Contains("闪")).ToList();

            if (dodgeCards.Count > 0)
            {
                // 这里应该由玩家选择是否出闪，暂时随机选择
                // 实际游戏中应该通过事件系统让玩家选择
                return false; // 假设玩家选择出闪，攻击被闪避
            }

            // 造成伤害
            var damage = engine.DealDamage(sourceId, targetId, 1, DamageType.Normal);
            
            // 如果使用了杀牌，移除并标记
            if (slashCard != null)
            {
                source.RemoveCard(slashCard.Id);
                source.HasUsedSlash = true;
            }

            return damage.Dealt;
        }

        /// <summary>
        /// 处理决斗
        /// </summary>
        public static bool ProcessDuel(ISGSKEngine engine, int sourceId, int targetId)
        {
            var source = engine.GetPlayer(sourceId);
            var target = engine.GetPlayer(targetId);

            if (source == null || target == null)
                return false;

            // 决斗逻辑：双方轮流出杀，先不出的一方受到伤害
            // 注意：实际游戏中，玩家可以选择不出杀，这里简化处理
            // 建议通过事件系统让玩家选择是否出杀
            bool sourceTurn = true;
            int maxRounds = 100; // 防止无限循环
            int round = 0;
            
            while (round < maxRounds)
            {
                round++;
                var currentPlayer = sourceTurn ? source : target;
                var slashCards = currentPlayer.Hand.Where(c => 
                    c.Type == CardType.Basic && 
                    c.Name.Contains("杀")).ToList();

                if (slashCards.Count == 0)
                {
                    // 当前玩家没有杀，受到伤害
                    var loser = sourceTurn ? source : target;
                    var winner = sourceTurn ? target : source;
                    engine.DealDamage(winner.Id, loser.Id, 1, DamageType.Normal);
                    return true;
                }

                // 这里应该由玩家选择是否出杀
                // 暂时假设玩家选择出杀
                var slashCard = slashCards[0];
                currentPlayer.RemoveCard(slashCard.Id);
                sourceTurn = !sourceTurn;
            }

            // 如果达到最大回合数，源玩家受到伤害（防止无限循环）
            engine.DealDamage(targetId, sourceId, 1, DamageType.Normal);
            return true;
        }
    }
}
