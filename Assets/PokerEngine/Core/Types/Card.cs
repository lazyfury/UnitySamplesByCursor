namespace PokerEngine.Core.Types
{
    /// <summary>
    /// 扑克牌
    /// </summary>
    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            string rankStr = Rank switch
            {
                Rank.Ace => "A",
                Rank.Jack => "J",
                Rank.Queen => "Q",
                Rank.King => "K",
                _ => ((int)Rank).ToString()
            };

            string suitStr = Suit switch
            {
                Suit.Spades => "♠",
                Suit.Hearts => "♥",
                Suit.Diamonds => "♦",
                Suit.Clubs => "♣",
                _ => ""
            };

            return $"{rankStr}{suitStr}";
        }

        public override bool Equals(object obj)
        {
            if (obj is Card other)
            {
                return Suit == other.Suit && Rank == other.Rank;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ((int)Suit << 8) | (int)Rank;
        }
    }
}
