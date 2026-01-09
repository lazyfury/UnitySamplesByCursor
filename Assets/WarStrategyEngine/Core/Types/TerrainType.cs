namespace WarStrategyEngine.Core
{
    /// <summary>
    /// 地形类型枚举
    /// </summary>
    public enum TerrainType
    {
        Plains,        // 平原
        Forest,        // 森林
        Mountain,      // 山地
        Water,         // 水域
        Desert,        // 沙漠
        Snow,          // 雪地
        Swamp,         // 沼泽
        Road,          // 道路
        City           // 城市
    }
    
    /// <summary>
    /// 地形类型扩展方法
    /// </summary>
    public static class TerrainTypeExtensions
    {
        /// <summary>
        /// 获取地形的移动消耗
        /// </summary>
        public static float GetMovementCost(this TerrainType terrain)
        {
            return terrain switch
            {
                TerrainType.Plains => 1.0f,
                TerrainType.Forest => 1.5f,
                TerrainType.Mountain => 3.0f,
                TerrainType.Water => 2.0f,
                TerrainType.Desert => 1.5f,
                TerrainType.Snow => 2.0f,
                TerrainType.Swamp => 2.5f,
                TerrainType.Road => 0.5f,
                TerrainType.City => 1.0f,
                _ => 1.0f
            };
        }
        
        /// <summary>
        /// 获取地形的防御加成
        /// </summary>
        public static float GetDefenseBonus(this TerrainType terrain)
        {
            return terrain switch
            {
                TerrainType.Forest => 0.2f,
                TerrainType.Mountain => 0.4f,
                TerrainType.City => 0.3f,
                TerrainType.Swamp => 0.1f,
                _ => 0.0f
            };
        }
        
        /// <summary>
        /// 检查地形是否可通行
        /// </summary>
        public static bool IsPassable(this TerrainType terrain, UnitType unitType)
        {
            if (terrain == TerrainType.Water)
            {
                return unitType == UnitType.Naval || unitType == UnitType.Air;
            }
            
            if (terrain == TerrainType.Mountain)
            {
                return unitType == UnitType.Air || unitType == UnitType.Infantry;
            }
            
            return true;
        }
    }
}
