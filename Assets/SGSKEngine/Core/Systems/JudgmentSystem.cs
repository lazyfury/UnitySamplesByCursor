using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Systems
{
    /// <summary>
    /// 判定系统
    /// </summary>
    public class JudgmentSystem
    {
        /// <summary>
        /// 执行判定
        /// </summary>
        public static Card ExecuteJudgment(ISGSKEngine engine, int playerId)
        {
            var player = engine.GetPlayer(playerId);
            if (player == null)
                return null;

            // 从牌堆顶抽取一张牌作为判定牌
            var judgmentCard = engine.Deck.Draw();
            if (judgmentCard == null)
                return null;

            // 判定牌进入弃牌堆
            engine.DiscardPile.Add(judgmentCard);

            return judgmentCard;
        }

        /// <summary>
        /// 判定乐不思蜀（红桃无效）
        /// </summary>
        public static bool CheckIndulgence(Card judgmentCard)
        {
            return judgmentCard.Suit != CardSuit.Heart;
        }

        /// <summary>
        /// 判定闪电（黑桃2-9的牌造成3点雷电伤害）
        /// </summary>
        public static bool CheckLightning(Card judgmentCard)
        {
            return judgmentCard.Suit == CardSuit.Spade &&
                   (int)judgmentCard.Rank >= 2 &&
                   (int)judgmentCard.Rank <= 9;
        }

        /// <summary>
        /// 判定兵粮寸断（梅花无效）
        /// </summary>
        public static bool CheckSupplyShortage(Card judgmentCard)
        {
            return judgmentCard.Suit != CardSuit.Club;
        }
    }
}
