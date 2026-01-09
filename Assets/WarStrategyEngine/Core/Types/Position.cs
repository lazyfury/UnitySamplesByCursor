using System;

namespace WarStrategyEngine.Core
{
    /// <summary>
    /// 二维坐标位置
    /// </summary>
    public struct Position : IEquatable<Position>
    {
        public int X { get; set; }
        public int Y { get; set; }
        
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }
        
        public override bool Equals(object obj)
        {
            return obj is Position other && Equals(other);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
        
        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }
        
        public static bool operator !=(Position left, Position right)
        {
            return !left.Equals(right);
        }
        
        public static Position operator +(Position left, Position right)
        {
            return new Position(left.X + right.X, left.Y + right.Y);
        }
        
        public static Position operator -(Position left, Position right)
        {
            return new Position(left.X - right.X, left.Y - right.Y);
        }
        
        public override string ToString()
        {
            return $"({X}, {Y})";
        }
        
        /// <summary>
        /// 计算曼哈顿距离
        /// </summary>
        public int ManhattanDistance(Position other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }
        
        /// <summary>
        /// 计算欧几里得距离
        /// </summary>
        public float EuclideanDistance(Position other)
        {
            int dx = X - other.X;
            int dy = Y - other.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
