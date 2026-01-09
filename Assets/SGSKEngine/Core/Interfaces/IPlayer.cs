using System.Collections.Generic;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Interfaces
{
    /// <summary>
    /// 玩家接口
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// 玩家ID
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// 玩家名称
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// 角色数据
        /// </summary>
        Character Character { get; }
        
        /// <summary>
        /// 当前血量
        /// </summary>
        int Hp { get; set; }
        
        /// <summary>
        /// 最大血量
        /// </summary>
        int MaxHp { get; }
        
        /// <summary>
        /// 手牌
        /// </summary>
        List<Card> Hand { get; }
        
        /// <summary>
        /// 装备区
        /// </summary>
        Dictionary<EquipmentSlot, Card> Equipments { get; }
        
        /// <summary>
        /// 判定区（延时锦囊）
        /// </summary>
        List<Card> JudgmentArea { get; }
        
        /// <summary>
        /// 玩家状态
        /// </summary>
        PlayerState State { get; set; }
        
        /// <summary>
        /// 标记（用于技能效果）
        /// </summary>
        Dictionary<string, object> Marks { get; }
        
        /// <summary>
        /// 是否已使用过杀
        /// </summary>
        bool HasUsedSlash { get; set; }
        
        /// <summary>
        /// 是否已使用过技能
        /// </summary>
        Dictionary<string, bool> SkillUsed { get; }

        // 方法
        void AddCard(Card card);
        bool RemoveCard(int cardId);
        Card GetCard(int cardId);
        bool HasCard(int cardId);
        bool EquipCard(Card card);
        bool UnequipCard(EquipmentSlot slot);
        Card GetEquipment(EquipmentSlot slot);
        void AddJudgmentCard(Card card);
        void RemoveJudgmentCard(int cardId);
        void AddMark(string key, object value);
        void RemoveMark(string key);
        bool HasMark(string key);
        T GetMark<T>(string key);
    }
}
