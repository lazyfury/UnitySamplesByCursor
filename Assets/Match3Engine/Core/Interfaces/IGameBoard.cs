using System.Collections.Generic;
using Match3Engine.Core.Types;

namespace Match3Engine.Core.Interfaces
{
    /// <summary>
    /// 游戏板接口
    /// </summary>
    public interface IGameBoard
    {
        /// <summary>
        /// 行数
        /// </summary>
        int Rows { get; }
        
        /// <summary>
        /// 列数
        /// </summary>
        int Cols { get; }
        
        /// <summary>
        /// 获取指定位置的宝石
        /// </summary>
        IGem GetGem(BoardPosition position);
        
        /// <summary>
        /// 设置指定位置的宝石
        /// </summary>
        void SetGem(BoardPosition position, IGem gem);
        
        /// <summary>
        /// 检查位置是否在有效范围内
        /// </summary>
        bool IsValidPosition(BoardPosition position);
        
        /// <summary>
        /// 交换两个位置的宝石
        /// </summary>
        void SwapGems(BoardPosition pos1, BoardPosition pos2);
        
        /// <summary>
        /// 移除指定位置的宝石
        /// </summary>
        void RemoveGem(BoardPosition position);
        
        /// <summary>
        /// 清空指定位置的宝石
        /// </summary>
        void ClearGem(BoardPosition position);
        
        /// <summary>
        /// 获取所有空位
        /// </summary>
        IEnumerable<BoardPosition> GetEmptyPositions();
        
        /// <summary>
        /// 检查位置是否为空
        /// </summary>
        bool IsEmpty(BoardPosition position);
        
        /// <summary>
        /// 随机生成一个宝石
        /// </summary>
        IGem GenerateRandomGem(BoardPosition position);
        
        /// <summary>
        /// 初始化游戏板（填充随机宝石，避免初始匹配）
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// 获取所有位置
        /// </summary>
        IEnumerable<BoardPosition> GetAllPositions();
    }
}
