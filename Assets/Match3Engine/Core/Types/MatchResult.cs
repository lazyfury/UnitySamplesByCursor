using System.Collections.Generic;

namespace Match3Engine.Core.Types
{
    /// <summary>
    /// 匹配结果
    /// </summary>
    public class MatchResult
    {
        /// <summary>
        /// 匹配到的位置列表
        /// </summary>
        public List<BoardPosition> MatchedPositions { get; set; }
        
        /// <summary>
        /// 匹配的宝石类型
        /// </summary>
        public GemType GemType { get; set; }
        
        /// <summary>
        /// 是否为横向匹配
        /// </summary>
        public bool IsHorizontal { get; set; }
        
        /// <summary>
        /// 匹配长度（连续匹配的宝石数量）
        /// </summary>
        public int MatchLength { get; set; }
        
        public MatchResult()
        {
            MatchedPositions = new List<BoardPosition>();
        }
        
        public MatchResult(GemType gemType, bool isHorizontal)
        {
            MatchedPositions = new List<BoardPosition>();
            GemType = gemType;
            IsHorizontal = isHorizontal;
            MatchLength = 0;
        }
    }
}
