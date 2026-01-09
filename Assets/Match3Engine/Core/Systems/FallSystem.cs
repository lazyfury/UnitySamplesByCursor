using System.Collections.Generic;
using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Types;

namespace Match3Engine.Core.Systems
{
    /// <summary>
    /// 下落系统
    /// </summary>
    public class FallSystem
    {
        /// <summary>
        /// 执行下落操作，返回是否有宝石下落
        /// </summary>
        public bool ExecuteFall(IGameBoard board)
        {
            bool hasFallen = false;
            
            // 从下往上，从右往左处理每一列
            for (int col = 0; col < board.Cols; col++)
            {
                // 找到该列中的所有空位
                var emptyPositions = new List<BoardPosition>();
                
                for (int row = board.Rows - 1; row >= 0; row--)
                {
                    var position = new BoardPosition(row, col);
                    if (board.IsEmpty(position))
                    {
                        emptyPositions.Add(position);
                    }
                }
                
                // 对每个空位，让上面的宝石下落
                foreach (var emptyPos in emptyPositions)
                {
                    // 从空位上方找到第一个非空宝石
                    IGem gemToFall = null;
                    BoardPosition gemPosition = emptyPos;
                    
                    for (int row = emptyPos.Row - 1; row >= 0; row--)
                    {
                        var checkPos = new BoardPosition(row, col);
                        var gem = board.GetGem(checkPos);
                        if (gem != null)
                        {
                            gemToFall = gem;
                            gemPosition = checkPos;
                            break;
                        }
                    }
                    
                    // 如果找到宝石，让它下落
                    if (gemToFall != null)
                    {
                        board.SwapGems(emptyPos, gemPosition);
                        hasFallen = true;
                    }
                }
            }
            
            return hasFallen;
        }
        
        /// <summary>
        /// 填充空位
        /// </summary>
        public void FillEmptySpaces(IGameBoard board)
        {
            foreach (var emptyPos in board.GetEmptyPositions())
            {
                var newGem = board.GenerateRandomGem(emptyPos);
                board.SetGem(emptyPos, newGem);
            }
        }
    }
}
