namespace WarStrategyEngine.Core.Interfaces
{
    /// <summary>
    /// 地图瓦片接口
    /// </summary>
    public interface ITile
    {
        /// <summary>
        /// 位置
        /// </summary>
        Position Position { get; }
        
        /// <summary>
        /// 地形类型
        /// </summary>
        TerrainType Terrain { get; set; }
        
        /// <summary>
        /// 移动消耗（基于地形）
        /// </summary>
        float MovementCost { get; }
        
        /// <summary>
        /// 防御加成（基于地形）
        /// </summary>
        float DefenseBonus { get; }
        
        /// <summary>
        /// 当前位于此瓦片的单位
        /// </summary>
        IUnit OccupyingUnit { get; set; }
        
        /// <summary>
        /// 是否可通行
        /// </summary>
        bool IsPassable { get; }
    }
}
