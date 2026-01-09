using System.Collections.Generic;
using System.Linq;
using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Types;

namespace Match3Engine.Core.Systems
{
    /// <summary>
    /// 匹配检测系统
    /// </summary>
    public class MatchDetector
    {
        /// <summary>
        /// 检测所有匹配
        /// </summary>
        public List<MatchResult> DetectAllMatches(IGameBoard board)
        {
            var matches = new List<MatchResult>();
            var checkedPositions = new HashSet<BoardPosition>();
            
            foreach (var position in board.GetAllPositions())
            {
                if (checkedPositions.Contains(position))
                    continue;
                
                var gem = board.GetGem(position);
                if (gem == null || gem.Type == GemType.None)
                    continue;
                
                // 检测水平匹配
                var horizontalMatch = DetectHorizontalMatch(board, position, gem.Type);
                if (horizontalMatch != null && horizontalMatch.MatchedPositions.Count >= 3)
                {
                    matches.Add(horizontalMatch);
                    foreach (var pos in horizontalMatch.MatchedPositions)
                    {
                        checkedPositions.Add(pos);
                    }
                }
                
                // 检测垂直匹配
                var verticalMatch = DetectVerticalMatch(board, position, gem.Type);
                if (verticalMatch != null && verticalMatch.MatchedPositions.Count >= 3)
                {
                    matches.Add(verticalMatch);
                    foreach (var pos in verticalMatch.MatchedPositions)
                    {
                        checkedPositions.Add(pos);
                    }
                }
            }
            
            return matches;
        }
        
        /// <summary>
        /// 检测指定位置的匹配（用于验证交换是否有效）
        /// </summary>
        public bool HasMatch(IGameBoard board, BoardPosition position)
        {
            var gem = board.GetGem(position);
            if (gem == null || gem.Type == GemType.None)
                return false;
            
            var horizontalMatch = DetectHorizontalMatch(board, position, gem.Type);
            if (horizontalMatch != null && horizontalMatch.MatchedPositions.Count >= 3)
                return true;
            
            var verticalMatch = DetectVerticalMatch(board, position, gem.Type);
            if (verticalMatch != null && verticalMatch.MatchedPositions.Count >= 3)
                return true;
            
            return false;
        }
        
        private MatchResult DetectHorizontalMatch(IGameBoard board, BoardPosition startPosition, GemType gemType)
        {
            var match = new MatchResult(gemType, true);
            match.MatchedPositions.Add(startPosition);
            
            // 向左检查
            int col = startPosition.Col - 1;
            while (col >= 0)
            {
                var pos = new BoardPosition(startPosition.Row, col);
                var gem = board.GetGem(pos);
                if (gem != null && gem.Type == gemType)
                {
                    match.MatchedPositions.Insert(0, pos);
                    col--;
                }
                else
                {
                    break;
                }
            }
            
            // 向右检查
            col = startPosition.Col + 1;
            while (col < board.Cols)
            {
                var pos = new BoardPosition(startPosition.Row, col);
                var gem = board.GetGem(pos);
                if (gem != null && gem.Type == gemType)
                {
                    match.MatchedPositions.Add(pos);
                    col++;
                }
                else
                {
                    break;
                }
            }
            
            match.MatchLength = match.MatchedPositions.Count;
            return match.MatchLength >= 3 ? match : null;
        }
        
        private MatchResult DetectVerticalMatch(IGameBoard board, BoardPosition startPosition, GemType gemType)
        {
            var match = new MatchResult(gemType, false);
            match.MatchedPositions.Add(startPosition);
            
            // 向上检查
            int row = startPosition.Row - 1;
            while (row >= 0)
            {
                var pos = new BoardPosition(row, startPosition.Col);
                var gem = board.GetGem(pos);
                if (gem != null && gem.Type == gemType)
                {
                    match.MatchedPositions.Insert(0, pos);
                    row--;
                }
                else
                {
                    break;
                }
            }
            
            // 向下检查
            row = startPosition.Row + 1;
            while (row < board.Rows)
            {
                var pos = new BoardPosition(row, startPosition.Col);
                var gem = board.GetGem(pos);
                if (gem != null && gem.Type == gemType)
                {
                    match.MatchedPositions.Add(pos);
                    row++;
                }
                else
                {
                    break;
                }
            }
            
            match.MatchLength = match.MatchedPositions.Count;
            return match.MatchLength >= 3 ? match : null;
        }
        
        /// <summary>
        /// 合并重叠的匹配结果（处理L型和T型匹配）
        /// </summary>
        public List<MatchResult> MergeMatches(List<MatchResult> matches)
        {
            if (matches == null || matches.Count == 0)
                return matches;
            
            var merged = new List<MatchResult>();
            var processedPositions = new HashSet<BoardPosition>();
            
            foreach (var match in matches)
            {
                var newMatch = new MatchResult(match.GemType, match.IsHorizontal);
                newMatch.MatchLength = match.MatchLength;
                
                foreach (var pos in match.MatchedPositions)
                {
                    if (!processedPositions.Contains(pos))
                    {
                        newMatch.MatchedPositions.Add(pos);
                        processedPositions.Add(pos);
                    }
                }
                
                if (newMatch.MatchedPositions.Count >= 3)
                {
                    merged.Add(newMatch);
                }
            }
            
            return merged;
        }
    }
}
