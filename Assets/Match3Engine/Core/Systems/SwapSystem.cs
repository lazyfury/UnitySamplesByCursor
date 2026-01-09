using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Types;

namespace Match3Engine.Core.Systems
{
    /// <summary>
    /// 交换系统
    /// </summary>
    public class SwapSystem
    {
        private MatchDetector _matchDetector;
        
        public SwapSystem()
        {
            _matchDetector = new MatchDetector();
        }
        
        /// <summary>
        /// 验证交换是否有效（两个位置必须相邻）
        /// </summary>
        public bool IsValidSwap(BoardPosition pos1, BoardPosition pos2)
        {
            return pos1.IsAdjacent(pos2);
        }
        
        /// <summary>
        /// 执行交换并检查是否产生匹配
        /// </summary>
        public SwapResult ExecuteSwap(IGameBoard board, BoardPosition pos1, BoardPosition pos2)
        {
            // 验证位置有效性
            if (!board.IsValidPosition(pos1) || !board.IsValidPosition(pos2))
            {
                return new SwapResult(false, false, "位置无效");
            }
            
            // 验证是否相邻
            if (!IsValidSwap(pos1, pos2))
            {
                return new SwapResult(false, false, "位置不相邻");
            }
            
            // 验证位置不为空
            if (board.IsEmpty(pos1) || board.IsEmpty(pos2))
            {
                return new SwapResult(false, false, "位置为空");
            }
            
            // 执行交换
            board.SwapGems(pos1, pos2);
            
            // 检查是否产生匹配
            bool hasMatch = _matchDetector.HasMatch(board, pos1) || 
                           _matchDetector.HasMatch(board, pos2);
            
            // 如果没有匹配，撤销交换
            if (!hasMatch)
            {
                board.SwapGems(pos1, pos2);
                return new SwapResult(false, false, "交换后未产生匹配");
            }
            
            return new SwapResult(true, true);
        }
    }
}
