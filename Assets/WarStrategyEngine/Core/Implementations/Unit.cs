using System;
using WarStrategyEngine.Core.Interfaces;
using WarStrategyEngine.Core.Systems;

namespace WarStrategyEngine.Core.Implementations
{
    /// <summary>
    /// 单位实现
    /// </summary>
    public class Unit : IUnit
    {
        private static int _nextId = 1;
        
        public int Id { get; private set; }
        public UnitType Type { get; private set; }
        public int OwnerId { get; private set; }
        public Position Position { get; set; }
        public UnitStats Stats { get; private set; }
        public bool IsAlive => Stats.CurrentHealth > 0;
        
        private IMap _map;
        
        public Unit(UnitType type, int ownerId, Position position, IMap map, UnitStats stats = null)
        {
            Id = _nextId++;
            Type = type;
            OwnerId = ownerId;
            Position = position;
            _map = map;
            
            if (stats == null)
            {
                Stats = CreateDefaultStats(type);
            }
            else
            {
                Stats = stats.Clone();
            }
        }
        
        private UnitStats CreateDefaultStats(UnitType type)
        {
            return type switch
            {
                UnitType.Infantry => new UnitStats(100, 15, 10, type.GetBaseMovement(), 1, 3),
                UnitType.Cavalry => new UnitStats(80, 20, 8, type.GetBaseMovement(), 1, 4),
                UnitType.Archer => new UnitStats(60, 12, 5, type.GetBaseMovement(), 3, 4),
                UnitType.Siege => new UnitStats(150, 30, 15, type.GetBaseMovement(), 4, 2),
                UnitType.Naval => new UnitStats(120, 18, 12, type.GetBaseMovement(), 2, 5),
                UnitType.Air => new UnitStats(70, 16, 6, type.GetBaseMovement(), 2, 6),
                UnitType.Worker => new UnitStats(40, 5, 3, type.GetBaseMovement(), 1, 2),
                UnitType.Hero => new UnitStats(200, 25, 15, type.GetBaseMovement(), 1, 5),
                _ => new UnitStats()
            };
        }
        
        public bool MoveTo(Position target)
        {
            if (!IsAlive)
                return false;
            
            if (Stats.HasMoved)
                return false;
            
            if (!_map.IsValidPosition(target))
                return false;
            
            var targetTile = _map.GetTile(target);
            if (targetTile == null || !targetTile.IsPassable)
                return false;
            
            if (targetTile.OccupyingUnit != null && targetTile.OccupyingUnit != this)
                return false;
            
            // 检查移动距离
            float distance = _map.GetDistance(Position, target);
            if (distance > Stats.Movement)
                return false;
            
            // 更新位置
            var oldTile = _map.GetTile(Position);
            if (oldTile != null)
            {
                oldTile.OccupyingUnit = null;
            }
            
            Position = target;
            targetTile.OccupyingUnit = this;
            Stats.HasMoved = true;
            
            return true;
        }
        
        public AttackResult Attack(IUnit target)
        {
            if (!IsAlive || target == null || !target.IsAlive)
                return new AttackResult { Success = false };
            
            var combatSystem = new CombatSystem();
            return combatSystem.ExecuteAttack(this, target, _map);
        }
        
        public void TakeDamage(int damage)
        {
            if (!IsAlive)
                return;
            
            Stats.CurrentHealth = Math.Max(0, Stats.CurrentHealth - damage);
            
            if (!IsAlive)
            {
                // 从地图上移除
                var tile = _map.GetTile(Position);
                if (tile != null)
                {
                    tile.OccupyingUnit = null;
                }
            }
        }
        
        public void Heal(int amount)
        {
            if (!IsAlive)
                return;
            
            Stats.CurrentHealth = Math.Min(Stats.MaxHealth, Stats.CurrentHealth + amount);
        }
        
        /// <summary>
        /// 重置回合状态
        /// </summary>
        public void ResetTurn()
        {
            Stats.ResetTurn();
        }
    }
}
