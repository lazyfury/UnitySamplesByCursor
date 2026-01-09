using System;
using System.Collections.Generic;
using System.Linq;
using WarStrategyEngine.Core.Interfaces;
using WarStrategyEngine.Core.Implementations;
using WarStrategyEngine.Core.Systems;

namespace WarStrategyEngine.Core.Implementations
{
    /// <summary>
    /// 游戏引擎核心实现
    /// </summary>
    public class GameEngine : IGameEngine
    {
        public GameState State { get; private set; }
        
        private IMap _map;
        private List<IPlayer> _players;
        private Dictionary<int, IUnit> _units;
        private int _currentPlayerIndex;
        private float _turnTimer;
        private const float TURN_DURATION = 30.0f; // 每回合30秒
        
        public GameEngine()
        {
            State = GameState.NotInitialized;
            _players = new List<IPlayer>();
            _units = new Dictionary<int, IUnit>();
            _currentPlayerIndex = 0;
            _turnTimer = 0;
        }
        
        public void Initialize(int mapWidth, int mapHeight)
        {
            State = GameState.Initializing;
            
            // 创建地图
            _map = new Map(mapWidth, mapHeight);
            
            // 创建玩家
            CreateDefaultPlayers();
            
            // 初始化单位
            InitializeUnits();
            
            State = GameState.WaitingForInput;
        }
        
        private void CreateDefaultPlayers()
        {
            _players.Clear();
            _players.Add(new Player(1, "Player 1", Faction.Player1, true));
            _players.Add(new Player(2, "Player 2", Faction.Player2, false));
        }
        
        private void InitializeUnits()
        {
            _units.Clear();
            
            // 为每个玩家创建初始单位
            foreach (var player in _players)
            {
                var startPos = GetStartPosition(player.Id);
                
                // 创建不同类型的单位
                var infantry = new Unit(UnitType.Infantry, player.Id, startPos, _map);
                var archer = new Unit(UnitType.Archer, player.Id, new Position(startPos.X + 1, startPos.Y), _map);
                var cavalry = new Unit(UnitType.Cavalry, player.Id, new Position(startPos.X, startPos.Y + 1), _map);
                
                // 设置地图瓦片的占用状态
                var infantryTile = _map.GetTile(startPos);
                if (infantryTile != null)
                {
                    infantryTile.OccupyingUnit = infantry;
                }
                
                var archerTile = _map.GetTile(new Position(startPos.X + 1, startPos.Y));
                if (archerTile != null)
                {
                    archerTile.OccupyingUnit = archer;
                }
                
                var cavalryTile = _map.GetTile(new Position(startPos.X, startPos.Y + 1));
                if (cavalryTile != null)
                {
                    cavalryTile.OccupyingUnit = cavalry;
                }
                
                _units[infantry.Id] = infantry;
                _units[archer.Id] = archer;
                _units[cavalry.Id] = cavalry;
                
                if (player is Player p)
                {
                    p.AddUnit(infantry);
                    p.AddUnit(archer);
                    p.AddUnit(cavalry);
                }
            }
        }
        
        private Position GetStartPosition(int playerId)
        {
            // 根据玩家ID分配起始位置
            if (playerId == 1)
            {
                return new Position(2, 2);
            }
            else
            {
                return new Position(_map.Width - 3, _map.Height - 3);
            }
        }
        
        public void Update(float deltaTime)
        {
            if (State != GameState.WaitingForInput && State != GameState.Processing)
                return;
            
            _turnTimer += deltaTime;
            
            // 检查回合是否结束
            if (_turnTimer >= TURN_DURATION)
            {
                EndTurn();
            }
            
            // 检查游戏是否结束
            CheckGameOver();
        }
        
        public IMap GetMap()
        {
            return _map;
        }
        
        public IEnumerable<IPlayer> GetPlayers()
        {
            return _players;
        }
        
        public IEnumerable<IUnit> GetUnits()
        {
            return _units.Values;
        }
        
        /// <summary>
        /// 获取当前玩家
        /// </summary>
        public IPlayer GetCurrentPlayer()
        {
            if (_players.Count == 0)
                return null;
            
            return _players[_currentPlayerIndex];
        }
        
        /// <summary>
        /// 结束当前回合
        /// </summary>
        public void EndTurn()
        {
            var currentPlayer = GetCurrentPlayer();
            if (currentPlayer is Player p)
            {
                p.ResetTurn();
            }
            
            // 切换到下一个玩家
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
            _turnTimer = 0;
            
            // 如果是AI玩家，执行AI逻辑
            var nextPlayer = GetCurrentPlayer();
            if (nextPlayer != null && !nextPlayer.IsHuman)
            {
                State = GameState.Processing;
                ExecuteAITurn(nextPlayer);
                State = GameState.WaitingForInput;
            }
        }
        
        private void ExecuteAITurn(IPlayer player)
        {
            // 简单的AI逻辑：移动单位并攻击
            foreach (var unit in player.Units)
            {
                if (!unit.IsAlive)
                    continue;
                
                // 寻找最近的敌人
                var nearestEnemy = FindNearestEnemy(unit, player.Id);
                if (nearestEnemy != null)
                {
                    // 尝试移动到敌人附近
                    var path = _map.FindPath(unit.Position, nearestEnemy.Position, unit);
                    if (path.Count > 1 && path.Count <= unit.Stats.Movement + 1)
                    {
                        unit.MoveTo(path[Math.Min(path.Count - 1, unit.Stats.Movement)]);
                    }
                    
                    // 如果敌人进入攻击范围，攻击
                    float distance = _map.GetDistance(unit.Position, nearestEnemy.Position);
                    if (distance <= unit.Stats.AttackRange)
                    {
                        unit.Attack(nearestEnemy);
                    }
                }
            }
        }
        
        private IUnit FindNearestEnemy(IUnit unit, int ownerId)
        {
            IUnit nearest = null;
            float minDistance = float.MaxValue;
            
            foreach (var otherUnit in _units.Values)
            {
                if (!otherUnit.IsAlive || otherUnit.OwnerId == ownerId)
                    continue;
                
                float distance = _map.GetDistance(unit.Position, otherUnit.Position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = otherUnit;
                }
            }
            
            return nearest;
        }
        
        private void CheckGameOver()
        {
            int alivePlayers = _players.Count(p => !p.IsDefeated);
            
            if (alivePlayers <= 1)
            {
                State = GameState.GameOver;
            }
        }
        
        /// <summary>
        /// 添加玩家
        /// </summary>
        public void AddPlayer(IPlayer player)
        {
            if (player != null && !_players.Contains(player))
            {
                _players.Add(player);
            }
        }
        
        /// <summary>
        /// 创建单位
        /// </summary>
        public IUnit CreateUnit(UnitType type, int ownerId, Position position)
        {
            var unit = new Unit(type, ownerId, position, _map);
            _units[unit.Id] = unit;
            
            var player = _players.FirstOrDefault(p => p.Id == ownerId);
            if (player is Player p)
            {
                p.AddUnit(unit);
            }
            
            return unit;
        }
        
        /// <summary>
        /// 暂停游戏
        /// </summary>
        public void Pause()
        {
            if (State == GameState.WaitingForInput || State == GameState.Processing)
            {
                State = GameState.Paused;
            }
        }
        
        /// <summary>
        /// 恢复游戏
        /// </summary>
        public void Resume()
        {
            if (State == GameState.Paused)
            {
                State = GameState.WaitingForInput;
            }
        }
    }
}
