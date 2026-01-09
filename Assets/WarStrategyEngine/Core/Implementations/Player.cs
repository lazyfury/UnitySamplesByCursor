using System.Collections.Generic;
using System.Linq;
using WarStrategyEngine.Core.Interfaces;
using WarStrategyEngine.Core.Systems;

namespace WarStrategyEngine.Core.Implementations
{
    /// <summary>
    /// 玩家实现
    /// </summary>
    public class Player : IPlayer
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Faction Faction { get; private set; }
        public ResourceManager Resources { get; private set; }
        public bool IsHuman { get; private set; }
        public bool IsDefeated => !Units.Any(u => u.IsAlive);
        
        private List<IUnit> _units;
        
        public IEnumerable<IUnit> Units => _units.Where(u => u.IsAlive);
        
        public Player(int id, string name, Faction faction, bool isHuman = true)
        {
            Id = id;
            Name = name;
            Faction = faction;
            IsHuman = isHuman;
            Resources = new ResourceManager();
            _units = new List<IUnit>();
            
            // 初始化默认资源
            InitializeDefaultResources();
        }
        
        private void InitializeDefaultResources()
        {
            Resources.SetResource(ResourceType.Gold, 1000);
            Resources.SetResource(ResourceType.Food, 500);
            Resources.SetResource(ResourceType.Wood, 300);
            Resources.SetResource(ResourceType.Stone, 200);
            Resources.SetResource(ResourceType.Iron, 100);
        }
        
        /// <summary>
        /// 添加单位
        /// </summary>
        public void AddUnit(IUnit unit)
        {
            if (unit != null && !_units.Contains(unit))
            {
                _units.Add(unit);
            }
        }
        
        /// <summary>
        /// 移除单位
        /// </summary>
        public void RemoveUnit(IUnit unit)
        {
            _units.Remove(unit);
        }
        
        /// <summary>
        /// 重置所有单位的回合状态
        /// </summary>
        public void ResetTurn()
        {
            foreach (var unit in _units)
            {
                if (unit is Unit u)
                {
                    u.ResetTurn();
                }
            }
        }
    }
}
