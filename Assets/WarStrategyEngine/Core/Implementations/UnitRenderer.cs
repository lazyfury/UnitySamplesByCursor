using System.Collections.Generic;
using UnityEngine;
using WarStrategyEngine.Core;
using WarStrategyEngine.Core.Interfaces;

namespace WarStrategyEngine.Core.Implementations
{
    /// <summary>
    /// 单位渲染器，用于在Unity中可视化游戏单位
    /// </summary>
    public class UnitRenderer : MonoBehaviour
    {
        [Header("渲染设置")]
        [Tooltip("单位大小（相对于瓦片）")]
        [Range(0.1f, 1f)]
        public float unitSize = 0.6f;
        
        [Tooltip("单位在瓦片上的高度偏移")]
        public float unitHeightOffset = 0.1f;
        
        [Tooltip("单位GameObject父节点")]
        public Transform unitParent;
        
        [Tooltip("是否使用Gizmos绘制（仅在Scene视图中可见）")]
        public bool useGizmos = false;
        
        private IGameEngine _gameEngine;
        private IMap _map;
        private TileRenderer _tileRenderer;
        private Dictionary<int, GameObject> _unitObjects = new Dictionary<int, GameObject>();
        private Dictionary<UnitType, Color> _unitTypeColors;
        private Dictionary<Faction, Color> _factionColors;
        
        // 用于跟踪同一位置上的单位数量，以便进行偏移
        private Dictionary<Position, List<IUnit>> _positionToUnits = new Dictionary<Position, List<IUnit>>();
        
        /// <summary>
        /// 初始化单位类型颜色映射
        /// </summary>
        private void InitializeUnitTypeColors()
        {
            _unitTypeColors = new Dictionary<UnitType, Color>
            {
                { UnitType.Infantry, new Color(0.8f, 0.8f, 0.8f) },      // 浅灰色 - 步兵
                { UnitType.Cavalry, new Color(0.9f, 0.7f, 0.5f) },      // 棕色 - 骑兵
                { UnitType.Archer, new Color(0.6f, 0.8f, 0.6f) },       // 浅绿色 - 弓箭手
                { UnitType.Siege, new Color(0.5f, 0.5f, 0.5f) },        // 深灰色 - 攻城器械
                { UnitType.Naval, new Color(0.3f, 0.5f, 0.9f) },        // 蓝色 - 海军
                { UnitType.Air, new Color(0.9f, 0.9f, 0.9f) },          // 白色 - 空军
                { UnitType.Worker, new Color(0.9f, 0.9f, 0.5f) },       // 黄色 - 工人
                { UnitType.Hero, new Color(1f, 0.8f, 0f) }              // 金色 - 英雄
            };
        }
        
        /// <summary>
        /// 初始化阵营颜色映射
        /// </summary>
        private void InitializeFactionColors()
        {
            _factionColors = new Dictionary<Faction, Color>
            {
                { Faction.Player1, Color.red },        // 红色 - 玩家1
                { Faction.Player2, Color.blue },       // 蓝色 - 玩家2
                { Faction.Player3, Color.green },      // 绿色 - 玩家3
                { Faction.Player4, Color.yellow },     // 黄色 - 玩家4
                { Faction.Neutral, Color.gray },       // 灰色 - 中立
                { Faction.AI, Color.magenta }          // 洋红色 - AI
            };
        }
        
        /// <summary>
        /// 设置游戏引擎和地图
        /// </summary>
        public void SetGameEngine(IGameEngine gameEngine, TileRenderer tileRenderer = null)
        {
            _gameEngine = gameEngine;
            _map = gameEngine?.GetMap();
            _tileRenderer = tileRenderer;
            
            InitializeUnitTypeColors();
            InitializeFactionColors();
            
            if (!useGizmos)
            {
                DrawUnits();
            }
        }
        
        /// <summary>
        /// 绘制所有单位
        /// </summary>
        public void DrawUnits()
        {
            if (_gameEngine == null)
                return;
            
            ClearUnits();
            
            // 首先收集所有单位，按位置分组
            _positionToUnits.Clear();
            foreach (var unit in _gameEngine.GetUnits())
            {
                if (unit.IsAlive)
                {
                    if (!_positionToUnits.ContainsKey(unit.Position))
                    {
                        _positionToUnits[unit.Position] = new List<IUnit>();
                    }
                    _positionToUnits[unit.Position].Add(unit);
                }
            }
            
            // 然后创建单位对象，同一位置上的单位会进行偏移
            foreach (var unit in _gameEngine.GetUnits())
            {
                if (unit.IsAlive)
                {
                    CreateUnitObject(unit);
                }
            }
        }
        
        /// <summary>
        /// 创建单位GameObject
        /// </summary>
        private void CreateUnitObject(IUnit unit)
        {
            // 获取玩家信息
            var player = GetPlayerById(unit.OwnerId);
            
            // 创建Sphere代表单位（也可以使用其他形状）
            GameObject unitObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            unitObj.name = $"Unit_{unit.Type}_{unit.Id}";
            
            // 获取瓦片大小
            float tileSize = _tileRenderer != null ? _tileRenderer.tileSize : 1f;
            
            // 计算单位位置（Unity 2D坐标系，位于瓦片中心上方）
            // 如果有多个单位在同一位置，进行偏移分布
            Vector3 basePos = new Vector3(
                unit.Position.X * tileSize + tileSize * 0.5f,
                unit.Position.Y * tileSize + tileSize * 0.5f,
                unitHeightOffset
            );
            
            Vector3 offset = CalculateUnitOffset(unit);
            Vector3 worldPos = basePos + offset;
            unitObj.transform.position = worldPos;
            unitObj.transform.rotation = Quaternion.identity;
            
            // 设置单位大小
            float actualSize = tileSize * unitSize;
            unitObj.transform.localScale = Vector3.one * actualSize;
            
            // 设置父节点
            if (unitParent != null)
            {
                unitObj.transform.SetParent(unitParent);
            }
            else
            {
                unitObj.transform.SetParent(transform);
            }
            
            // 设置颜色（结合单位类型和阵营）
            var renderer = unitObj.GetComponent<Renderer>();
            if (renderer != null)
            {
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
                
                // 获取基础颜色（单位类型）
                Color baseColor = GetUnitTypeColor(unit.Type);
                
                // 如果有玩家信息，使用阵营颜色进行混合或替换
                if (player != null)
                {
                    Color factionColor = GetFactionColor(player.Faction);
                    // 混合两种颜色（可以根据需要调整混合方式）
                    material.color = Color.Lerp(baseColor, factionColor, 0.5f);
                }
                else
                {
                    material.color = baseColor;
                }
                
                renderer.material = material;
            }
            
            // 移除碰撞器（可选）
            var collider = unitObj.GetComponent<Collider>();
            if (collider != null)
            {
                DestroyImmediate(collider);
            }
            
            _unitObjects[unit.Id] = unitObj;
        }
        
        /// <summary>
        /// 根据ID获取玩家
        /// </summary>
        private IPlayer GetPlayerById(int playerId)
        {
            if (_gameEngine == null)
                return null;
            
            foreach (var player in _gameEngine.GetPlayers())
            {
                if (player.Id == playerId)
                    return player;
            }
            
            return null;
        }
        
        /// <summary>
        /// 计算单位在同一位置上的偏移（避免重叠）
        /// </summary>
        private Vector3 CalculateUnitOffset(IUnit unit)
        {
            if (!_positionToUnits.ContainsKey(unit.Position))
                return Vector3.zero;
            
            var unitsAtPosition = _positionToUnits[unit.Position];
            if (unitsAtPosition.Count <= 1)
                return Vector3.zero;
            
            // 找到当前单位在列表中的索引
            int index = unitsAtPosition.IndexOf(unit);
            if (index < 0)
                return Vector3.zero;
            
            // 计算偏移：将单位分布在圆形或网格上
            float tileSize = _tileRenderer != null ? _tileRenderer.tileSize : 1f;
            float offsetRadius = tileSize * 0.25f; // 偏移半径（不超过瓦片的1/4）
            
            int totalUnits = unitsAtPosition.Count;
            float offsetX;
            float offsetY = 0f;
            
            // 如果只有2个单位，左右分布
            if (totalUnits == 2)
            {
                offsetX = (index == 0) ? -offsetRadius * 0.5f : offsetRadius * 0.5f;
                return new Vector3(offsetX, offsetY, 0f);
            }
            
            // 3个或更多单位，按圆形分布
            float angleStep = 360f / totalUnits;
            float angle = index * angleStep * Mathf.Deg2Rad;
            offsetX = Mathf.Cos(angle) * offsetRadius;
            offsetY = Mathf.Sin(angle) * offsetRadius;
            
            return new Vector3(offsetX, offsetY, 0f);
        }
        
        /// <summary>
        /// 更新单位位置
        /// </summary>
        public void UpdateUnitPosition(IUnit unit)
        {
            // 重新计算位置到单位的映射
            _positionToUnits.Clear();
            foreach (var u in _gameEngine.GetUnits())
            {
                if (u.IsAlive)
                {
                    if (!_positionToUnits.ContainsKey(u.Position))
                    {
                        _positionToUnits[u.Position] = new List<IUnit>();
                    }
                    _positionToUnits[u.Position].Add(u);
                }
            }
            
            if (!_unitObjects.TryGetValue(unit.Id, out GameObject unitObj) || unitObj == null)
            {
                // 如果对象不存在，创建它
                if (unit.IsAlive)
                {
                    CreateUnitObject(unit);
                }
                return;
            }
            
            if (!unit.IsAlive)
            {
                // 单位已死亡，移除对象
                DestroyUnitObject(unit.Id);
                return;
            }
            
            // 更新位置（包括偏移）
            float tileSize = _tileRenderer != null ? _tileRenderer.tileSize : 1f;
            Vector3 basePos = new Vector3(
                unit.Position.X * tileSize + tileSize * 0.5f,
                unit.Position.Y * tileSize + tileSize * 0.5f,
                unitHeightOffset
            );
            
            Vector3 offset = CalculateUnitOffset(unit);
            Vector3 newPos = basePos + offset;
            
            // 可以添加平滑移动
            unitObj.transform.position = newPos;
        }
        
        /// <summary>
        /// 清除所有单位对象
        /// </summary>
        public void ClearUnits()
        {
            foreach (var unitObj in _unitObjects.Values)
            {
                if (unitObj != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(unitObj);
                    }
                    else
                    {
                        DestroyImmediate(unitObj);
                    }
                }
            }
            _unitObjects.Clear();
            _positionToUnits.Clear();
        }
        
        /// <summary>
        /// 销毁单个单位对象
        /// </summary>
        private void DestroyUnitObject(int unitId)
        {
            if (_unitObjects.TryGetValue(unitId, out GameObject unitObj))
            {
                if (unitObj != null)
                {
                    if (Application.isPlaying)
                    {
                        Destroy(unitObj);
                    }
                    else
                    {
                        DestroyImmediate(unitObj);
                    }
                }
                _unitObjects.Remove(unitId);
            }
        }
        
        /// <summary>
        /// 获取单位类型颜色
        /// </summary>
        public Color GetUnitTypeColor(UnitType unitType)
        {
            if (_unitTypeColors == null)
                InitializeUnitTypeColors();
            
            return _unitTypeColors.TryGetValue(unitType, out Color color) ? color : Color.white;
        }
        
        /// <summary>
        /// 获取阵营颜色
        /// </summary>
        public Color GetFactionColor(Faction faction)
        {
            if (_factionColors == null)
                InitializeFactionColors();
            
            return _factionColors.TryGetValue(faction, out Color color) ? color : Color.gray;
        }
        
        /// <summary>
        /// 使用Gizmos绘制（在Scene视图中可见）
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!useGizmos || _gameEngine == null)
                return;
            
            if (_unitTypeColors == null)
                InitializeUnitTypeColors();
            if (_factionColors == null)
                InitializeFactionColors();
            
            // 重新计算位置到单位的映射（用于Gizmos）
            _positionToUnits.Clear();
            foreach (var unit in _gameEngine.GetUnits())
            {
                if (unit.IsAlive)
                {
                    if (!_positionToUnits.ContainsKey(unit.Position))
                    {
                        _positionToUnits[unit.Position] = new List<IUnit>();
                    }
                    _positionToUnits[unit.Position].Add(unit);
                }
            }
            
            float tileSize = _tileRenderer != null ? _tileRenderer.tileSize : 1f;
            float actualSize = tileSize * unitSize;
            
            foreach (var unit in _gameEngine.GetUnits())
            {
                if (!unit.IsAlive)
                    continue;
                
                // 计算世界位置（包括偏移）
                Vector3 basePos = new Vector3(
                    unit.Position.X * tileSize + tileSize * 0.5f,
                    unit.Position.Y * tileSize + tileSize * 0.5f,
                    unitHeightOffset
                );
                
                Vector3 offset = CalculateUnitOffset(unit);
                Vector3 center = basePos + offset;
                
                // 获取颜色
                var player = GetPlayerById(unit.OwnerId);
                Color baseColor = GetUnitTypeColor(unit.Type);
                
                if (player != null)
                {
                    Color factionColor = GetFactionColor(player.Faction);
                    Gizmos.color = Color.Lerp(baseColor, factionColor, 0.5f);
                }
                else
                {
                    Gizmos.color = baseColor;
                }
                
                // 绘制单位（使用球体）
                Gizmos.DrawSphere(center, actualSize * 0.5f);
            }
        }
        
        /// <summary>
        /// 清理资源
        /// </summary>
        private void OnDestroy()
        {
            ClearUnits();
        }
    }
}
