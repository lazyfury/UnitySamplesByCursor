using System.Collections.Generic;
using System.Linq;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Systems
{
    /// <summary>
    /// 卡牌效果系统
    /// </summary>
    public class CardEffectSystem
    {
        private static Dictionary<string, ICardEffect> _cardEffects = new Dictionary<string, ICardEffect>();

        /// <summary>
        /// 注册卡牌效果
        /// </summary>
        public static void RegisterCardEffect(ICardEffect effect)
        {
            _cardEffects[effect.CardName] = effect;
        }

        /// <summary>
        /// 执行卡牌效果
        /// </summary>
        public static bool ExecuteCardEffect(ISGSKEngine engine, IPlayer player, Card card, List<IPlayer> targets)
        {
            if (_cardEffects.ContainsKey(card.Name))
            {
                var effect = _cardEffects[card.Name];
                if (effect.CanPlay(engine, player, targets))
                {
                    effect.Execute(engine, player, targets, card);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查是否可以打出卡牌
        /// </summary>
        public static bool CanPlayCard(ISGSKEngine engine, IPlayer player, Card card, List<IPlayer> targets)
        {
            if (_cardEffects.ContainsKey(card.Name))
            {
                var effect = _cardEffects[card.Name];
                return effect.CanPlay(engine, player, targets);
            }

            // 默认处理
            switch (card.Type)
            {
                case CardType.Basic:
                    return CanPlayBasicCard(engine, player, card, targets);
                case CardType.Trick:
                    return CanPlayTrickCard(engine, player, card, targets);
                case CardType.Equipment:
                    return CanPlayEquipmentCard(engine, player, card);
                default:
                    return false;
            }
        }

        private static bool CanPlayBasicCard(ISGSKEngine engine, IPlayer player, Card card, List<IPlayer> targets)
        {
            if (card.Name.Contains("杀"))
            {
                if (player.HasUsedSlash)
                    return false;
                
                if (targets == null || targets.Count == 0)
                    return false;

                var target = targets[0];
                return engine.IsInRange(player.Id, target.Id);
            }

            if (card.Name.Contains("桃"))
            {
                // 桃可以在出牌阶段对自己使用，或在濒死时使用
                return player.Hp < player.MaxHp || player.State == PlayerState.Dying;
            }

            return true;
        }

        private static bool CanPlayTrickCard(ISGSKEngine engine, IPlayer player, Card card, List<IPlayer> targets)
        {
            // 锦囊牌的具体判断逻辑
            return true;
        }

        private static bool CanPlayEquipmentCard(ISGSKEngine engine, IPlayer player, Card card)
        {
            // 装备牌可以直接装备
            return true;
        }
    }
}
