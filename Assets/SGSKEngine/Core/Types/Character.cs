using System.Collections.Generic;

namespace SGSKEngine.Core.Types
{
    /// <summary>
    /// 角色数据
    /// </summary>
    public class Character
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 最大血量
        /// </summary>
        public int MaxHp { get; set; }
        
        /// <summary>
        /// 势力（主公、忠臣、反贼、内奸）
        /// </summary>
        public string Faction { get; set; }
        
        /// <summary>
        /// 技能列表
        /// </summary>
        public List<string> Skills { get; set; }
        
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        public Character(int id, string name, int maxHp, string faction = "", string gender = "男")
        {
            Id = id;
            Name = name;
            MaxHp = maxHp;
            Faction = faction;
            Gender = gender;
            Skills = new List<string>();
        }
    }
}
