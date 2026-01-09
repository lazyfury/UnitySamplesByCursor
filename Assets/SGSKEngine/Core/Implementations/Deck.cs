using System;
using System.Collections.Generic;
using System.Linq;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Implementations
{
    /// <summary>
    /// 牌堆实现
    /// </summary>
    public class Deck : IDeck
    {
        private List<Card> _cards;
        private Random _random;

        public int Count => _cards.Count;
        public bool IsEmpty => _cards.Count == 0;

        public Deck()
        {
            _cards = new List<Card>();
            _random = new Random();
        }

        public void Shuffle()
        {
            // Fisher-Yates洗牌算法
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var temp = _cards[i];
                _cards[i] = _cards[j];
                _cards[j] = temp;
            }
        }

        public Card Draw()
        {
            if (_cards.Count == 0)
                return null;

            var card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }

        public List<Card> Draw(int count)
        {
            var drawn = new List<Card>();
            for (int i = 0; i < count && _cards.Count > 0; i++)
            {
                drawn.Add(Draw());
            }
            return drawn;
        }

        public void AddCard(Card card)
        {
            if (card != null)
                _cards.Add(card);
        }

        public void AddCards(List<Card> cards)
        {
            if (cards != null)
                _cards.AddRange(cards);
        }

        public Card Peek()
        {
            return _cards.Count > 0 ? _cards[0] : null;
        }

        public void InitializeStandardDeck()
        {
            _cards.Clear();
            int cardId = 1;

            // 基本牌
            // 杀：30张
            for (int i = 0; i < 30; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                _cards.Add(new Card(cardId++, "杀", CardType.Basic, suit, rank));
            }

            // 闪：15张
            for (int i = 0; i < 15; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                _cards.Add(new Card(cardId++, "闪", CardType.Basic, suit, rank));
            }

            // 桃：8张
            for (int i = 0; i < 8; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                _cards.Add(new Card(cardId++, "桃", CardType.Basic, suit, rank));
            }

            // 锦囊牌
            // 过河拆桥：6张
            for (int i = 0; i < 6; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                _cards.Add(new Card(cardId++, "过河拆桥", CardType.Trick, suit, rank));
            }

            // 顺手牵羊：5张
            for (int i = 0; i < 5; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                _cards.Add(new Card(cardId++, "顺手牵羊", CardType.Trick, suit, rank));
            }

            // 无中生有：4张
            for (int i = 0; i < 4; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                _cards.Add(new Card(cardId++, "无中生有", CardType.Trick, suit, rank));
            }

            // 决斗：3张
            for (int i = 0; i < 3; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                _cards.Add(new Card(cardId++, "决斗", CardType.Trick, suit, rank));
            }

            // 万箭齐发：1张
            _cards.Add(new Card(cardId++, "万箭齐发", CardType.Trick, CardSuit.Heart, CardRank.King));

            // 南蛮入侵：3张
            for (int i = 0; i < 3; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                _cards.Add(new Card(cardId++, "南蛮入侵", CardType.Trick, suit, rank));
            }

            // 桃园结义：1张
            _cards.Add(new Card(cardId++, "桃园结义", CardType.Trick, CardSuit.Heart, CardRank.King));

            // 五谷丰登：2张
            for (int i = 0; i < 2; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                _cards.Add(new Card(cardId++, "五谷丰登", CardType.Trick, suit, rank));
            }

            // 无懈可击：7张
            for (int i = 0; i < 7; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                _cards.Add(new Card(cardId++, "无懈可击", CardType.Trick, suit, rank));
            }

            // 延时锦囊
            // 乐不思蜀：3张
            for (int i = 0; i < 3; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                var card = new Card(cardId++, "乐不思蜀", CardType.Trick, suit, rank);
                card.IsDelayedTrick = true;
                _cards.Add(card);
            }

            // 闪电：1张
            var lightning = new Card(cardId++, "闪电", CardType.Trick, CardSuit.Spade, CardRank.Ace);
            lightning.IsDelayedTrick = true;
            _cards.Add(lightning);

            // 兵粮寸断：2张
            for (int i = 0; i < 2; i++)
            {
                var suit = (CardSuit)(i % 4);
                var rank = (CardRank)((i / 4) % 13 + 1);
                var card = new Card(cardId++, "兵粮寸断", CardType.Trick, suit, rank);
                card.IsDelayedTrick = true;
                _cards.Add(card);
            }

            // 装备牌（简化处理，实际应该更详细）
            // 这里只添加少量示例装备
            var weapon = new Card(cardId++, "诸葛连弩", CardType.Equipment, CardSuit.Spade, CardRank.Ace);
            weapon.EquipmentSlot = EquipmentSlot.Weapon;
            weapon.AttackRange = 1;
            _cards.Add(weapon);

            Shuffle();
        }
    }
}
