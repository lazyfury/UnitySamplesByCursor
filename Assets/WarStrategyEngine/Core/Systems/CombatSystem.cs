using System;
using WarStrategyEngine.Core.Interfaces;

namespace WarStrategyEngine.Core.Systems
{
    /// <summary>
    /// 战斗系统
    /// </summary>
    public class CombatSystem
    {
        private readonly Random _random;
        
        public CombatSystem(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }
        
        /// <summary>
        /// 计算伤害
        /// </summary>
        public int CalculateDamage(IUnit attacker, IUnit defender, ITile defenderTile)
        {
            int baseDamage = attacker.Stats.Attack;
            int defense = defender.Stats.Defense;
            
            // 应用地形防御加成
            float defenseBonus = defenderTile?.DefenseBonus ?? 0f;
            int adjustedDefense = (int)(defense * (1 + defenseBonus));
            
            // 基础伤害计算：攻击力 - 防御力
            int damage = Math.Max(1, baseDamage - adjustedDefense);
            
            // 添加随机波动（±20%）
            float variance = (float)(_random.NextDouble() * 0.4 - 0.2); // -0.2 到 0.2
            damage = (int)(damage * (1 + variance));
            
            return Math.Max(1, damage);
        }
        
        /// <summary>
        /// 执行攻击
        /// </summary>
        public AttackResult ExecuteAttack(IUnit attacker, IUnit defender, IMap map)
        {
            var result = new AttackResult();
            
            if (attacker == null || defender == null)
            {
                result.Success = false;
                return result;
            }
            
            if (!attacker.IsAlive || !defender.IsAlive)
            {
                result.Success = false;
                return result;
            }
            
            // 检查攻击范围
            float distance = map.GetDistance(attacker.Position, defender.Position);
            if (distance > attacker.Stats.AttackRange)
            {
                result.Success = false;
                return result;
            }
            
            // 检查是否已攻击
            if (attacker.Stats.HasAttacked)
            {
                result.Success = false;
                return result;
            }
            
            var defenderTile = map.GetTile(defender.Position);
            
            // 计算伤害
            result.Damage = CalculateDamage(attacker, defender, defenderTile);
            result.Success = true;
            
            // 造成伤害
            defender.TakeDamage(result.Damage);
            
            // 检查是否击杀
            result.TargetKilled = !defender.IsAlive;
            
            // 如果目标存活且是近战攻击，执行反击
            if (!result.TargetKilled && attacker.Stats.AttackRange == 1 && defender.Stats.AttackRange == 1)
            {
                result.CounterAttacked = true;
                var attackerTile = map.GetTile(attacker.Position);
                result.CounterDamage = CalculateDamage(defender, attacker, attackerTile);
                attacker.TakeDamage(result.CounterDamage);
            }
            
            // 标记已攻击
            attacker.Stats.HasAttacked = true;
            
            return result;
        }
        
        /// <summary>
        /// 检查是否可以攻击目标
        /// </summary>
        public bool CanAttack(IUnit attacker, IUnit target, IMap map)
        {
            if (attacker == null || target == null)
                return false;
            
            if (!attacker.IsAlive || !target.IsAlive)
                return false;
            
            if (attacker.Stats.HasAttacked)
                return false;
            
            float distance = map.GetDistance(attacker.Position, target.Position);
            return distance <= attacker.Stats.AttackRange;
        }
    }
}
