namespace SGSKEngine.Core.Types
{
    /// <summary>
    /// 卡牌
    /// </summary>
    public class Card
    {
        /// <summary>
        /// 卡牌ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 卡牌名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 卡牌类型
        /// </summary>
        public CardType Type { get; set; }
        
        /// <summary>
        /// 花色
        /// </summary>
        public CardSuit Suit { get; set; }
        
        /// <summary>
        /// 点数
        /// </summary>
        public CardRank Rank { get; set; }
        
        /// <summary>
        /// 是否为延时锦囊
        /// </summary>
        public bool IsDelayedTrick { get; set; }
        
        /// <summary>
        /// 装备槽位（仅装备牌有效）
        /// </summary>
        public EquipmentSlot? EquipmentSlot { get; set; }
        
        /// <summary>
        /// 攻击范围（仅武器有效）
        /// </summary>
        public int? AttackRange { get; set; }
        
        /// <summary>
        /// 防御值（仅防具有效）
        /// </summary>
        public int? DefenseValue { get; set; }
        
        /// <summary>
        /// 距离修正（仅坐骑有效，+1或-1）
        /// </summary>
        public int? DistanceModifier { get; set; }

        public Card(int id, string name, CardType type, CardSuit suit, CardRank rank)
        {
            Id = id;
            Name = name;
            Type = type;
            Suit = suit;
            Rank = rank;
            IsDelayedTrick = false;
        }

        public override string ToString()
        {
            return $"{Name}({Suit}{Rank})";
        }

        public override bool Equals(object obj)
        {
            if (obj is Card other)
            {
                return Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
