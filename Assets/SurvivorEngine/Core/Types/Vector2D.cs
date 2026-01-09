using System;

namespace SurvivorEngine.Core.Types
{
    /// <summary>
    /// 2D向量结构，用于位置和方向计算
    /// </summary>
    [Serializable]
    public struct Vector2D
    {
        public float X;
        public float Y;

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float Magnitude => (float)Math.Sqrt(X * X + Y * Y);
        public float SqrMagnitude => X * X + Y * Y;
        
        public Vector2D Normalized
        {
            get
            {
                float mag = Magnitude;
                if (mag > 0.0001f)
                    return new Vector2D(X / mag, Y / mag);
                return new Vector2D(0, 0);
            }
        }

        public static Vector2D Zero => new Vector2D(0, 0);
        public static Vector2D One => new Vector2D(1, 1);

        public static Vector2D operator +(Vector2D a, Vector2D b) => new Vector2D(a.X + b.X, a.Y + b.Y);
        public static Vector2D operator -(Vector2D a, Vector2D b) => new Vector2D(a.X - b.X, a.Y - b.Y);
        public static Vector2D operator *(Vector2D a, float scalar) => new Vector2D(a.X * scalar, a.Y * scalar);
        public static Vector2D operator /(Vector2D a, float scalar) => new Vector2D(a.X / scalar, a.Y / scalar);
        public static Vector2D operator -(Vector2D a) => new Vector2D(-a.X, -a.Y);

        public static float Distance(Vector2D a, Vector2D b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static float SqrDistance(Vector2D a, Vector2D b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;
            return dx * dx + dy * dy;
        }

        public override string ToString() => $"({X:F2}, {Y:F2})";
    }
}
