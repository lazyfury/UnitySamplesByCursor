using System;

namespace Match3Engine.Core.Types
{
    /// <summary>
    /// 游戏板位置坐标
    /// </summary>
    public struct BoardPosition : IEquatable<BoardPosition>
    {
        public int Row { get; set; }
        public int Col { get; set; }
        
        public BoardPosition(int row, int col)
        {
            Row = row;
            Col = col;
        }
        
        public bool Equals(BoardPosition other)
        {
            return Row == other.Row && Col == other.Col;
        }
        
        public override bool Equals(object obj)
        {
            return obj is BoardPosition other && Equals(other);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Col);
        }
        
        public static bool operator ==(BoardPosition left, BoardPosition right)
        {
            return left.Equals(right);
        }
        
        public static bool operator !=(BoardPosition left, BoardPosition right)
        {
            return !left.Equals(right);
        }
        
        public static BoardPosition operator +(BoardPosition left, BoardPosition right)
        {
            return new BoardPosition(left.Row + right.Row, left.Col + right.Col);
        }
        
        public static BoardPosition operator -(BoardPosition left, BoardPosition right)
        {
            return new BoardPosition(left.Row - right.Row, left.Col - right.Col);
        }
        
        public override string ToString()
        {
            return $"({Row}, {Col})";
        }
        
        /// <summary>
        /// 检查是否相邻（上下左右）
        /// </summary>
        public bool IsAdjacent(BoardPosition other)
        {
            int rowDiff = Math.Abs(Row - other.Row);
            int colDiff = Math.Abs(Col - other.Col);
            return (rowDiff == 1 && colDiff == 0) || (rowDiff == 0 && colDiff == 1);
        }
        
        /// <summary>
        /// 获取曼哈顿距离
        /// </summary>
        public int ManhattanDistance(BoardPosition other)
        {
            return Math.Abs(Row - other.Row) + Math.Abs(Col - other.Col);
        }
    }
}
