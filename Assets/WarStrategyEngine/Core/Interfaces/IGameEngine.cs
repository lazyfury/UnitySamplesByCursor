using System;
using System.Collections.Generic;

namespace WarStrategyEngine.Core.Interfaces
{
    /// <summary>
    /// 游戏引擎核心接口，定义引擎无关的核心功能
    /// </summary>
    public interface IGameEngine
    {
        /// <summary>
        /// 游戏状态
        /// </summary>
        GameState State { get; }
        
        /// <summary>
        /// 初始化游戏
        /// </summary>
        void Initialize(int mapWidth, int mapHeight);
        
        /// <summary>
        /// 更新游戏逻辑（每帧或每回合调用）
        /// </summary>
        void Update(float deltaTime);
        
        /// <summary>
        /// 获取地图
        /// </summary>
        IMap GetMap();
        
        /// <summary>
        /// 获取所有玩家
        /// </summary>
        IEnumerable<IPlayer> GetPlayers();
        
        /// <summary>
        /// 获取所有单位
        /// </summary>
        IEnumerable<IUnit> GetUnits();
        
        /// <summary>
        /// 获取当前玩家
        /// </summary>
        IPlayer GetCurrentPlayer();
    }
}
