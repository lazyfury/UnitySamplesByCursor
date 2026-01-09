using System.Collections.Generic;
using UnityEngine;
using WarStrategyEngine.Core;
using WarStrategyEngine.Core.Interfaces;

namespace WarStrategyEngine.Core.Implementations
{
    /// <summary>
    /// 地图瓦片渲染器，用于在Unity中可视化地图
    /// </summary>
    public class TileRenderer : MonoBehaviour
    {
        [Header("渲染设置")]
        [Tooltip("瓦片大小")]
        public float tileSize = 1f;
        
        [Tooltip("瓦片之间的间距")]
        [Range(0f, 0.5f)]
        public float tileGap = 0.05f;
        
        [Tooltip("是否显示瓦片边界")]
        public bool showTileBorders = true;
        
        [Tooltip("边界线条宽度")]
        public float borderWidth = 0.05f;
        
        [Tooltip("是否使用Gizmos绘制（仅在Scene视图中可见）")]
        public bool useGizmos = false;
        
        [Tooltip("瓦片GameObject父节点")]
        public Transform tileParent;
        
        private IMap _map;
        private Dictionary<Position, GameObject> _tileObjects = new Dictionary<Position, GameObject>();
        private Dictionary<TerrainType, Color> _terrainColors;
        
        /// <summary>
        /// 初始化地形颜色映射
        /// </summary>
        private void InitializeTerrainColors()
        {
            _terrainColors = new Dictionary<TerrainType, Color>
            {
                { TerrainType.Plains, new Color(0.8f, 0.9f, 0.6f) },      // 浅绿色 - 平原
                { TerrainType.Forest, new Color(0.2f, 0.6f, 0.2f) },      // 深绿色 - 森林
                { TerrainType.Mountain, new Color(0.5f, 0.5f, 0.5f) },    // 灰色 - 山地
                { TerrainType.Water, new Color(0.3f, 0.5f, 0.9f) },       // 蓝色 - 水域
                { TerrainType.Desert, new Color(0.9f, 0.8f, 0.5f) },      // 黄色 - 沙漠
                { TerrainType.Snow, new Color(0.95f, 0.95f, 1f) },        // 白色 - 雪地
                { TerrainType.Swamp, new Color(0.4f, 0.5f, 0.3f) },       // 棕绿色 - 沼泽
                { TerrainType.Road, new Color(0.6f, 0.6f, 0.6f) },        // 灰色 - 道路
                { TerrainType.City, new Color(0.7f, 0.7f, 0.7f) }         // 浅灰色 - 城市
            };
        }
        
        /// <summary>
        /// 设置要渲染的地图
        /// </summary>
        public void SetMap(IMap map)
        {
            _map = map;
            InitializeTerrainColors();
            
            if (!useGizmos)
            {
                DrawTiles();
            }
        }
        
        /// <summary>
        /// 绘制所有瓦片
        /// </summary>
        public void DrawTiles()
        {
            if (_map == null)
                return;
            
            ClearTiles();
            
            for (int x = 0; x < _map.Width; x++)
            {
                for (int y = 0; y < _map.Height; y++)
                {
                    var position = new Position(x, y);
                    var tile = _map.GetTile(position);
                    if (tile != null)
                    {
                        CreateTileObject(tile);
                    }
                }
            }
        }
        
        /// <summary>
        /// 创建瓦片GameObject
        /// </summary>
        private void CreateTileObject(ITile tile)
        {
            // 创建Quad网格
            GameObject tileObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            tileObj.name = $"Tile_{tile.Position.X}_{tile.Position.Y}";
            
            // 设置位置（将地图坐标转换为Unity 2D世界坐标）
            // Unity 2D坐标系：X向右，Y向上，Z=0
            // 考虑gap后，瓦片的实际尺寸会减小，但中心位置保持不变
            float actualTileSize = tileSize - tileGap;
            Vector3 worldPos = new Vector3(
                tile.Position.X * tileSize + tileSize * 0.5f,
                tile.Position.Y * tileSize + tileSize * 0.5f,
                0f
            );
            tileObj.transform.position = worldPos;
            // 2D中Quad默认朝向相机，不需要旋转
            tileObj.transform.rotation = Quaternion.identity;
            // 使用实际尺寸（减去gap）
            tileObj.transform.localScale = Vector3.one * actualTileSize;
            
            // 设置父节点
            if (tileParent != null)
            {
                tileObj.transform.SetParent(tileParent);
            }
            else
            {
                tileObj.transform.SetParent(transform);
            }
            
            // 设置颜色（使用Unity 2D Shader或Unlit Shader）
            var renderer = tileObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                // 尝试使用Unlit/Transparent shader（适合2D）
                Shader shader = Shader.Find("Unlit/Color");
                if (shader == null)
                {
                    shader = Shader.Find("Sprites/Default");
                }
                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                }
                
                var material = new Material(shader);
                if (_terrainColors.TryGetValue(tile.Terrain, out Color color))
                {
                    material.color = color;
                }
                renderer.material = material;
            }
            
            // 移除碰撞器（可选，如果需要点击检测可以保留）
            var collider = tileObj.GetComponent<Collider>();
            if (collider != null)
            {
                DestroyImmediate(collider);
            }
            
            _tileObjects[tile.Position] = tileObj;
        }
        
        /// <summary>
        /// 清除所有已创建的瓦片对象
        /// </summary>
        public void ClearTiles()
        {
            foreach (var tileObj in _tileObjects.Values)
            {
                if (tileObj != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(tileObj);
                    }
                    else
                    {
                        DestroyImmediate(tileObj);
                    }
                }
            }
            _tileObjects.Clear();
        }
        
        /// <summary>
        /// 获取地形颜色
        /// </summary>
        public Color GetTerrainColor(TerrainType terrain)
        {
            if (_terrainColors == null)
                InitializeTerrainColors();
            
            return _terrainColors.TryGetValue(terrain, out Color color) ? color : Color.white;
        }
        
        /// <summary>
        /// 更新瓦片颜色（当地形改变时调用）
        /// </summary>
        public void UpdateTileColor(Position position)
        {
            if (_map == null || !_tileObjects.TryGetValue(position, out GameObject tileObj))
                return;
            
            var tile = _map.GetTile(position);
            if (tile != null && tileObj != null)
            {
                var renderer = tileObj.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    if (_terrainColors.TryGetValue(tile.Terrain, out Color color))
                    {
                        renderer.material.color = color;
                    }
                }
            }
        }
        
        /// <summary>
        /// 使用Gizmos绘制（在Scene视图中可见）
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!useGizmos || _map == null)
                return;
            
            if (_terrainColors == null)
                InitializeTerrainColors();
            
            for (int x = 0; x < _map.Width; x++)
            {
                for (int y = 0; y < _map.Height; y++)
                {
                    var position = new Position(x, y);
                    var tile = _map.GetTile(position);
                    if (tile != null)
                    {
                        // 计算世界位置（Unity 2D坐标系）
                        Vector3 center = new Vector3(
                            position.X * tileSize + tileSize * 0.5f,
                            position.Y * tileSize + tileSize * 0.5f,
                            0f
                        );
                        
                        // 计算实际瓦片尺寸（减去gap）
                        float actualTileSize = tileSize - tileGap;
                        
                        // 绘制瓦片
                        Color color = GetTerrainColor(tile.Terrain);
                        Gizmos.color = color;
                        
                        // 2D中瓦片深度很小，使用实际尺寸
                        Vector3 size = new Vector3(actualTileSize, actualTileSize, 0.1f);
                        Gizmos.DrawCube(center, size);
                        
                        // 绘制边界
                        if (showTileBorders)
                        {
                            Gizmos.color = Color.black;
                            float halfActualSize = actualTileSize * 0.5f;
                            
                            // 上边界（Y+方向）
                            Vector3 topBorderPos = new Vector3(center.x, center.y + halfActualSize, center.z);
                            Vector3 topBorderSize = new Vector3(actualTileSize, borderWidth, 0.1f);
                            Gizmos.DrawCube(topBorderPos, topBorderSize);
                            
                            // 下边界（Y-方向）
                            Vector3 bottomBorderPos = new Vector3(center.x, center.y - halfActualSize, center.z);
                            Vector3 bottomBorderSize = new Vector3(actualTileSize, borderWidth, 0.1f);
                            Gizmos.DrawCube(bottomBorderPos, bottomBorderSize);
                            
                            // 左边界（X-方向）
                            Vector3 leftBorderPos = new Vector3(center.x - halfActualSize, center.y, center.z);
                            Vector3 leftBorderSize = new Vector3(borderWidth, actualTileSize, 0.1f);
                            Gizmos.DrawCube(leftBorderPos, leftBorderSize);
                            
                            // 右边界（X+方向）
                            Vector3 rightBorderPos = new Vector3(center.x + halfActualSize, center.y, center.z);
                            Vector3 rightBorderSize = new Vector3(borderWidth, actualTileSize, 0.1f);
                            Gizmos.DrawCube(rightBorderPos, rightBorderSize);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 清理资源
        /// </summary>
        private void OnDestroy()
        {
            ClearTiles();
        }
    }
}
