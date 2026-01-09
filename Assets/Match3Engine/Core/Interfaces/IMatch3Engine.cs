using System;
using Match3Engine.Core.Types;

namespace Match3Engine.Core.Interfaces
{
    /// <summary>
    /// 三消游戏引擎接口
    /// </summary>
    public interface IMatch3Engine
    {
        /// <summary>
        /// 游戏状态
        /// </summary>
        GameState State { get; }
        
        /// <summary>
        /// 游戏板
        /// </summary>
        IGameBoard Board { get; }
        
        /// <summary>
        /// 当前分数
        /// </summary>
        int Score { get; }
        
        /// <summary>
        /// 当前连击数
        /// </summary>
        int ComboCount { get; }
        
        /// <summary>
        /// 剩余移动次数（如果有限制）
        /// </summary>
        int RemainingMoves { get; set; }
        
        /// <summary>
        /// 初始化游戏
        /// </summary>
        void Initialize(int rows, int cols);
        
        /// <summary>
        /// 尝试交换两个位置的宝石
        /// </summary>
        SwapResult TrySwap(BoardPosition pos1, BoardPosition pos2);
        
        /// <summary>
        /// 更新游戏逻辑
        /// </summary>
        void Update(float deltaTime);
        
        /// <summary>
        /// 暂停游戏
        /// </summary>
        void Pause();
        
        /// <summary>
        /// 恢复游戏
        /// </summary>
        void Resume();
        
        /// <summary>
        /// 重置游戏
        /// </summary>
        void Reset();
        
        /// <summary>
        /// 匹配事件（当检测到匹配时触发）
        /// </summary>
        event Action<MatchResult> OnMatchFound;
        
        /// <summary>
        /// 消除完成事件
        /// </summary>
        event Action OnEliminationComplete;
        
        /// <summary>
        /// 下落完成事件
        /// </summary>
        event Action OnFallComplete;
        
        /// <summary>
        /// 分数变化事件
        /// </summary>
        event Action<int> OnScoreChanged;
        
        /// <summary>
        /// 游戏结束事件
        /// </summary>
        event Action OnGameOver;
    }
}
