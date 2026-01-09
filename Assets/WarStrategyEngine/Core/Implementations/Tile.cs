using WarStrategyEngine.Core.Interfaces;

namespace WarStrategyEngine.Core.Implementations
{
    /// <summary>
    /// 地图瓦片实现
    /// </summary>
    public class Tile : ITile
    {
        public Position Position { get; private set; }
        public TerrainType Terrain { get; set; }
        public IUnit OccupyingUnit { get; set; }
        
        public float MovementCost => Terrain.GetMovementCost();
        public float DefenseBonus => Terrain.GetDefenseBonus();
        public bool IsPassable => true; // 具体通行性由地形和单位类型决定
        
        public Tile(Position position, TerrainType terrain = TerrainType.Plains)
        {
            Position = position;
            Terrain = terrain;
            OccupyingUnit = null;
        }
        
        /// <summary>
        /// 检查指定单位类型是否可以通行
        /// </summary>
        public bool IsPassableFor(UnitType unitType)
        {
            return Terrain.IsPassable(unitType);
        }
    }
}
