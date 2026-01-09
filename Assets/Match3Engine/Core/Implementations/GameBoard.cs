using System;
using System.Collections.Generic;
using System.Linq;
using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Types;

namespace Match3Engine.Core.Implementations
{
    /// <summary>
    /// 游戏板实现
    /// </summary>
    public class GameBoard : IGameBoard
    {
        private IGem[,] _grid;
        private Random _random;
        
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        
        public GameBoard(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            _grid = new IGem[rows, cols];
            _random = new Random();
        }
        
        public IGem GetGem(BoardPosition position)
        {
            if (!IsValidPosition(position))
                return null;
            
            return _grid[position.Row, position.Col];
        }
        
        public void SetGem(BoardPosition position, IGem gem)
        {
            if (!IsValidPosition(position))
                return;
            
            _grid[position.Row, position.Col] = gem;
            if (gem != null)
            {
                gem.Position = position;
            }
        }
        
        public bool IsValidPosition(BoardPosition position)
        {
            return position.Row >= 0 && position.Row < Rows &&
                   position.Col >= 0 && position.Col < Cols;
        }
        
        public void SwapGems(BoardPosition pos1, BoardPosition pos2)
        {
            if (!IsValidPosition(pos1) || !IsValidPosition(pos2))
                return;
            
            var gem1 = _grid[pos1.Row, pos1.Col];
            var gem2 = _grid[pos2.Row, pos2.Col];
            
            _grid[pos1.Row, pos1.Col] = gem2;
            _grid[pos2.Row, pos2.Col] = gem1;
            
            if (gem1 != null)
            {
                gem1.Position = pos2;
            }
            if (gem2 != null)
            {
                gem2.Position = pos1;
            }
        }
        
        public void RemoveGem(BoardPosition position)
        {
            if (!IsValidPosition(position))
                return;
            
            _grid[position.Row, position.Col] = null;
        }
        
        public void ClearGem(BoardPosition position)
        {
            RemoveGem(position);
        }
        
        public IEnumerable<BoardPosition> GetEmptyPositions()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    if (_grid[row, col] == null)
                    {
                        yield return new BoardPosition(row, col);
                    }
                }
            }
        }
        
        public bool IsEmpty(BoardPosition position)
        {
            if (!IsValidPosition(position))
                return false;
            
            return _grid[position.Row, position.Col] == null;
        }
        
        public IGem GenerateRandomGem(BoardPosition position)
        {
            // 排除None和Special类型
            var validTypes = Enum.GetValues(typeof(GemType))
                .Cast<GemType>()
                .Where(t => t != GemType.None && t != GemType.Special)
                .ToArray();
            
            var randomType = validTypes[_random.Next(validTypes.Length)];
            return new Gem(randomType, position);
        }
        
        public void Initialize()
        {
            // 清空游戏板
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    _grid[row, col] = null;
                }
            }
            
            // 填充宝石，避免初始匹配
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    var position = new BoardPosition(row, col);
                    var gem = GenerateGemWithoutInitialMatch(position);
                    SetGem(position, gem);
                }
            }
        }
        
        private IGem GenerateGemWithoutInitialMatch(BoardPosition position)
        {
            var validTypes = Enum.GetValues(typeof(GemType))
                .Cast<GemType>()
                .Where(t => t != GemType.None && t != GemType.Special)
                .ToList();
            
            // 尝试生成宝石，确保不会形成初始匹配
            for (int attempts = 0; attempts < 50; attempts++)
            {
                var randomType = validTypes[_random.Next(validTypes.Count)];
                var gem = new Gem(randomType, position);
                
                if (!WouldCreateInitialMatch(position, randomType))
                {
                    return gem;
                }
            }
            
            // 如果尝试多次都失败，返回随机类型
            return GenerateRandomGem(position);
        }
        
        private bool WouldCreateInitialMatch(BoardPosition position, GemType gemType)
        {
            // 检查水平方向
            int horizontalCount = 1;
            if (position.Col >= 1 && GetGem(new BoardPosition(position.Row, position.Col - 1))?.Type == gemType)
            {
                horizontalCount++;
                if (position.Col >= 2 && GetGem(new BoardPosition(position.Row, position.Col - 2))?.Type == gemType)
                {
                    horizontalCount++;
                }
            }
            if (position.Col < Cols - 1 && GetGem(new BoardPosition(position.Row, position.Col + 1))?.Type == gemType)
            {
                horizontalCount++;
            }
            
            // 检查垂直方向
            int verticalCount = 1;
            if (position.Row >= 1 && GetGem(new BoardPosition(position.Row - 1, position.Col))?.Type == gemType)
            {
                verticalCount++;
                if (position.Row >= 2 && GetGem(new BoardPosition(position.Row - 2, position.Col))?.Type == gemType)
                {
                    verticalCount++;
                }
            }
            if (position.Row < Rows - 1 && GetGem(new BoardPosition(position.Row + 1, position.Col))?.Type == gemType)
            {
                verticalCount++;
            }
            
            return horizontalCount >= 3 || verticalCount >= 3;
        }
        
        public IEnumerable<BoardPosition> GetAllPositions()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    yield return new BoardPosition(row, col);
                }
            }
        }
    }
}
