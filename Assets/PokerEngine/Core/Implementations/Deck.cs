using System;
using System.Collections.Generic;
using System.Linq;
using PokerEngine.Core.Interfaces;
using PokerEngine.Core.Types;

namespace PokerEngine.Core.Implementations
{
    /// <summary>
    /// 牌组实现
    /// </summary>
    public class Deck : IDeck
    {
        private List<Card> _cards;
        private Random _random;

        public int RemainingCards => _cards.Count;

        public Deck()
        {
            _random = new Random();
            Reset();
        }

        public void Reset()
        {
            _cards = new List<Card>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    _cards.Add(new Card(suit, rank));
                }
            }
            Shuffle();
        }

        public void Shuffle()
        {
            // Fisher-Yates洗牌算法
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                Card temp = _cards[i];
                _cards[i] = _cards[j];
                _cards[j] = temp;
            }
        }

        public Card DealCard()
        {
            if (_cards.Count == 0)
                throw new InvalidOperationException("牌组已空，无法发牌");

            Card card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }

        public List<Card> DealCards(int count)
        {
            var cards = new List<Card>();
            for (int i = 0; i < count; i++)
            {
                cards.Add(DealCard());
            }
            return cards;
        }
    }
}
