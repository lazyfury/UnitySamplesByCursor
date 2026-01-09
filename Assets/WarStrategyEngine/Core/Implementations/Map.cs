using System;
using System.Collections.Generic;
using System.Linq;
using WarStrategyEngine.Core.Interfaces;
using WarStrategyEngine.Core.Systems;

namespace WarStrategyEngine.Core.Implementations
{
    /// <summary>
    /// 地图实现
    /// </summary>
    public class Map : IMap
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        private ITile[,] _tiles;
        
        // 噪声图生成参数
        private float _noiseScale = 10f;
        private int _noiseOctaves = 4;
        private float _noisePersistence = 0.5f;
        private float _noiseLacunarity = 2f;
        private int _noiseSeed = 0;
        
        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            _tiles = new ITile[width, height];
            
            InitializeMap();
        }
        
        /// <summary>
        /// 使用自定义噪声参数创建地图
        /// </summary>
        public Map(int width, int height, float noiseScale, int noiseOctaves = 4, float noisePersistence = 0.5f, float noiseLacunarity = 2f, int noiseSeed = 0)
        {
            Width = width;
            Height = height;
            _tiles = new ITile[width, height];
            
            SetNoiseParameters(noiseScale, noiseOctaves, noisePersistence, noiseLacunarity, noiseSeed);
            InitializeMap();
        }
        
        /// <summary>
        /// 使用噪声图初始化地图
        /// </summary>
        private void InitializeMap()
        {
            var random = new Random(_noiseSeed == 0 ? Environment.TickCount : _noiseSeed);
            float offsetX = (float)random.NextDouble() * 10000f;
            float offsetY = (float)random.NextDouble() * 10000f;
            
            // 生成高度噪声图
            float[,] heightNoise = NoiseMapGenerator.GenerateNoiseMap(
                Width, Height, 
                _noiseScale, 
                _noiseOctaves, 
                _noisePersistence, 
                _noiseLacunarity,
                offsetX, 
                offsetY
            );
            
            // 生成湿度噪声图（使用不同的偏移）
            float[,] moistureNoise = NoiseMapGenerator.GenerateNoiseMap(
                Width, Height, 
                _noiseScale * 1.5f, 
                _noiseOctaves, 
                _noisePersistence, 
                _noiseLacunarity,
                offsetX + 1000f, 
                offsetY + 1000f
            );
            
            // 根据噪声值分配地形
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var position = new Position(x, y);
                    TerrainType terrain = GetTerrainFromNoise(heightNoise[x, y], moistureNoise[x, y]);
                    _tiles[x, y] = new Tile(position, terrain);
                }
            }
        }
        
        /// <summary>
        /// 根据高度和湿度噪声值确定地形类型
        /// </summary>
        private TerrainType GetTerrainFromNoise(float height, float moisture)
        {
            // 根据高度和湿度组合决定地形
            // 高度分层：0-0.3 水域/沼泽, 0.3-0.5 平原/沙漠, 0.5-0.7 森林/平原, 0.7-1.0 山地/雪地
            // 湿度影响：低湿度 -> 沙漠/雪地, 高湿度 -> 森林/沼泽
            
            if (height < 0.3f)
            {
                // 低海拔区域
                if (moisture > 0.6f)
                    return TerrainType.Swamp;  // 高湿度 -> 沼泽
                else
                    return TerrainType.Water;  // 低湿度 -> 水域
            }
            else if (height < 0.5f)
            {
                // 中低海拔区域
                if (moisture < 0.3f)
                    return TerrainType.Desert;  // 低湿度 -> 沙漠
                else
                    return TerrainType.Plains;  // 高湿度 -> 平原
            }
            else if (height < 0.7f)
            {
                // 中海拔区域
                if (moisture > 0.5f)
                    return TerrainType.Forest;  // 高湿度 -> 森林
                else
                    return TerrainType.Plains;  // 低湿度 -> 平原
            }
            else
            {
                // 高海拔区域
                if (moisture < 0.4f)
                    return TerrainType.Snow;    // 低湿度 -> 雪地
                else
                    return TerrainType.Mountain; // 高湿度 -> 山地
            }
        }
        
        /// <summary>
        /// 设置噪声生成参数
        /// </summary>
        public void SetNoiseParameters(float scale = 10f, int octaves = 4, float persistence = 0.5f, float lacunarity = 2f, int seed = 0)
        {
            _noiseScale = scale;
            _noiseOctaves = octaves;
            _noisePersistence = persistence;
            _noiseLacunarity = lacunarity;
            _noiseSeed = seed;
        }
        
        public ITile GetTile(Position position)
        {
            if (!IsValidPosition(position))
                return null;
            
            return _tiles[position.X, position.Y];
        }
        
        public ITile GetTile(int x, int y)
        {
            return GetTile(new Position(x, y));
        }
        
        public void SetTerrain(Position position, TerrainType terrain)
        {
            var tile = GetTile(position);
            if (tile != null)
            {
                tile.Terrain = terrain;
            }
        }
        
        public bool IsValidPosition(Position position)
        {
            return position.X >= 0 && position.X < Width &&
                   position.Y >= 0 && position.Y < Height;
        }
        
        public IEnumerable<Position> GetNeighbors(Position position)
        {
            var neighbors = new List<Position>();
            
            // 四方向相邻
            var directions = new[]
            {
                new Position(0, -1),  // 上
                new Position(1, 0),   // 右
                new Position(0, 1),   // 下
                new Position(-1, 0)   // 左
            };
            
            foreach (var dir in directions)
            {
                var neighbor = position + dir;
                if (IsValidPosition(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }
            
            return neighbors;
        }
        
        public float GetDistance(Position from, Position to)
        {
            // 使用欧几里得距离
            return from.EuclideanDistance(to);
        }
        
        public List<Position> FindPath(Position from, Position to, IUnit unit)
        {
            return Pathfinding.FindPath(this, from, to, unit);
        }
    }
}
