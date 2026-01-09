using System.Collections.Generic;

namespace WarStrategyEngine.Core.Interfaces
{
    /// <summary>
    /// 地图接口
    /// </summary>
    public interface IMap
    {
        /// <summary>
        /// 地图宽度
        /// </summary>
        int Width { get; }
        
        /// <summary>
        /// 地图高度
        /// </summary>
        int Height { get; }
        
        /// <summary>
        /// 获取指定位置的地形
        /// </summary>
        ITile GetTile(Position position);
        
        /// <summary>
        /// 获取指定位置的地形
        /// </summary>
        ITile GetTile(int x, int y);
        
        /// <summary>
        /// 设置地形类型
        /// </summary>
        void SetTerrain(Position position, TerrainType terrain);
        
        /// <summary>
        /// 检查位置是否在地图范围内
        /// </summary>
        bool IsValidPosition(Position position);
        
        /// <summary>
        /// 获取相邻位置
        /// </summary>
        IEnumerable<Position> GetNeighbors(Position position);
        
        /// <summary>
        /// 计算两点间的距离
        /// </summary>
        float GetDistance(Position from, Position to);
        
        /// <summary>
        /// 查找路径
        /// </summary>
        List<Position> FindPath(Position from, Position to, IUnit unit);
    }
}
