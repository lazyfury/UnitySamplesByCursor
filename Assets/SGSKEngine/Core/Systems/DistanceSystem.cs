using System.Collections.Generic;
using System.Linq;
using SGSKEngine.Core.Interfaces;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Systems
{
    /// <summary>
    /// 距离系统
    /// </summary>
    public class DistanceSystem
    {
        /// <summary>
        /// 计算两个玩家之间的距离
        /// </summary>
        public static int CalculateDistance(ISGSKEngine engine, int fromId, int toId)
        {
            var players = engine.AlivePlayers;
            var fromIndex = players.FindIndex(p => p.Id == fromId);
            var toIndex = players.FindIndex(p => p.Id == toId);

            if (fromIndex == -1 || toIndex == -1)
                return int.MaxValue;

            if (fromIndex == toIndex)
                return 0;

            // 计算座位距离（顺时针和逆时针的最小值）
            int totalPlayers = players.Count;
            int clockwiseDistance = (toIndex - fromIndex + totalPlayers) % totalPlayers;
            int counterClockwiseDistance = (fromIndex - toIndex + totalPlayers) % totalPlayers;
            int seatDistance = System.Math.Min(clockwiseDistance, counterClockwiseDistance);

            // 应用距离修正
            var fromPlayer = engine.GetPlayer(fromId);
            var toPlayer = engine.GetPlayer(toId);

            // 进攻马（+1马）增加攻击距离
            var offensiveHorse = fromPlayer.GetEquipment(EquipmentSlot.OffensiveHorse);
            if (offensiveHorse != null && offensiveHorse.DistanceModifier.HasValue)
            {
                seatDistance -= offensiveHorse.DistanceModifier.Value;
            }

            // 防御马（-1马）增加防御距离
            var defensiveHorse = toPlayer.GetEquipment(EquipmentSlot.DefensiveHorse);
            if (defensiveHorse != null && defensiveHorse.DistanceModifier.HasValue)
            {
                seatDistance += defensiveHorse.DistanceModifier.Value;
            }

            // 武器增加攻击范围
            var weapon = fromPlayer.GetEquipment(EquipmentSlot.Weapon);
            if (weapon != null && weapon.AttackRange.HasValue)
            {
                // 武器范围影响攻击距离
                // 这里简化处理，实际应该更复杂
            }

            return System.Math.Max(1, seatDistance); // 最小距离为1
        }

        /// <summary>
        /// 检查是否在攻击范围内
        /// </summary>
        public static bool IsInAttackRange(ISGSKEngine engine, int fromId, int toId)
        {
            var fromPlayer = engine.GetPlayer(fromId);
            if (fromPlayer == null)
                return false;

            int distance = CalculateDistance(engine, fromId, toId);
            int attackRange = 1; // 默认攻击范围为1

            // 检查武器
            var weapon = fromPlayer.GetEquipment(EquipmentSlot.Weapon);
            if (weapon != null && weapon.AttackRange.HasValue)
            {
                attackRange = weapon.AttackRange.Value;
            }

            return distance <= attackRange;
        }
    }
}
