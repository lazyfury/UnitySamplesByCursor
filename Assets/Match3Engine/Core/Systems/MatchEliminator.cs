using System.Collections.Generic;
using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Types;

namespace Match3Engine.Core.Systems
{
    /// <summary>
    /// 消除系统
    /// </summary>
    public class MatchEliminator
    {
        /// <summary>
        /// 消除所有匹配的宝石
        /// </summary>
        public HashSet<BoardPosition> EliminateMatches(IGameBoard board, List<MatchResult> matches)
        {
            var eliminatedPositions = new HashSet<BoardPosition>();
            
            foreach (var match in matches)
            {
                foreach (var position in match.MatchedPositions)
                {
                    if (board.IsValidPosition(position))
                    {
                        board.RemoveGem(position);
                        eliminatedPositions.Add(position);
                    }
                }
            }
            
            return eliminatedPositions;
        }
        
        /// <summary>
        /// 计算匹配得分
        /// </summary>
        public int CalculateScore(List<MatchResult> matches, int comboMultiplier = 1)
        {
            int baseScore = 0;
            
            foreach (var match in matches)
            {
                // 基础分数：匹配长度 * 10
                int matchScore = match.MatchLength * 10;
                
                // 额外奖励：超过3个的每个宝石额外10分
                if (match.MatchLength > 3)
                {
                    matchScore += (match.MatchLength - 3) * 10;
                }
                
                baseScore += matchScore;
            }
            
            return baseScore * comboMultiplier;
        }
    }
}
