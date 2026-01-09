using System.Collections.Generic;
using System.Linq;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Implementations
{
    /// <summary>
    /// 玩家实现
    /// </summary>
    public class Player : IPlayer
    {
        public int Id { get; }
        public string Name { get; }
        public Character Character { get; }
        public int Hp { get; set; }
        public int MaxHp => Character.MaxHp;
        public List<Card> Hand { get; }
        public Dictionary<EquipmentSlot, Card> Equipments { get; }
        public List<Card> JudgmentArea { get; }
        public PlayerState State { get; set; }
        public Dictionary<string, object> Marks { get; }
        public bool HasUsedSlash { get; set; }
        public Dictionary<string, bool> SkillUsed { get; }

        public Player(int id, string name, Character character)
        {
            Id = id;
            Name = name;
            Character = character;
            Hp = character.MaxHp;
            Hand = new List<Card>();
            Equipments = new Dictionary<EquipmentSlot, Card>();
            JudgmentArea = new List<Card>();
            State = PlayerState.Alive;
            Marks = new Dictionary<string, object>();
            SkillUsed = new Dictionary<string, bool>();
            HasUsedSlash = false;
        }

        public void AddCard(Card card)
        {
            if (card != null)
                Hand.Add(card);
        }

        public bool RemoveCard(int cardId)
        {
            var card = Hand.FirstOrDefault(c => c.Id == cardId);
            if (card != null)
            {
                Hand.Remove(card);
                return true;
            }
            return false;
        }

        public Card GetCard(int cardId)
        {
            return Hand.FirstOrDefault(c => c.Id == cardId);
        }

        public bool HasCard(int cardId)
        {
            return Hand.Any(c => c.Id == cardId);
        }

        public bool EquipCard(Card card)
        {
            if (card?.EquipmentSlot == null)
                return false;

            var slot = card.EquipmentSlot.Value;
            
            // 如果该槽位已有装备，先卸下
            if (Equipments.ContainsKey(slot))
            {
                UnequipCard(slot);
            }

            Equipments[slot] = card;
            return true;
        }

        public bool UnequipCard(EquipmentSlot slot)
        {
            if (Equipments.ContainsKey(slot))
            {
                Equipments.Remove(slot);
                return true;
            }
            return false;
        }

        public Card GetEquipment(EquipmentSlot slot)
        {
            return Equipments.ContainsKey(slot) ? Equipments[slot] : null;
        }

        public void AddJudgmentCard(Card card)
        {
            if (card != null)
                JudgmentArea.Add(card);
        }

        public void RemoveJudgmentCard(int cardId)
        {
            var card = JudgmentArea.FirstOrDefault(c => c.Id == cardId);
            if (card != null)
                JudgmentArea.Remove(card);
        }

        public void AddMark(string key, object value)
        {
            Marks[key] = value;
        }

        public void RemoveMark(string key)
        {
            Marks.Remove(key);
        }

        public bool HasMark(string key)
        {
            return Marks.ContainsKey(key);
        }

        public T GetMark<T>(string key)
        {
            if (Marks.ContainsKey(key) && Marks[key] is T)
                return (T)Marks[key];
            return default(T);
        }
    }
}
