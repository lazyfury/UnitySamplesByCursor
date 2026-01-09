using System;

namespace SurvivorEngine.Core.Types
{
    /// <summary>
    /// 投射物数据
    /// </summary>
    [Serializable]
    public class ProjectileData
    {
        public int Id { get; set; }
        public Vector2D Position { get; set; }
        public Vector2D Direction { get; set; }
        public float Speed { get; set; }
        public float Damage { get; set; }
        public float Lifetime { get; set; }
        public float CurrentLifetime { get; set; }
        public WeaponType WeaponType { get; set; }
        public float Range { get; set; }
        public Vector2D StartPosition { get; set; }

        public ProjectileData()
        {
            Id = 0;
            Position = Vector2D.Zero;
            Direction = Vector2D.Zero;
            Speed = 5f;
            Damage = 10f;
            Lifetime = 5f;
            CurrentLifetime = 0f;
            WeaponType = WeaponType.None;
            Range = 10f;
            StartPosition = Vector2D.Zero;
        }

        public bool IsExpired => CurrentLifetime >= Lifetime || 
                                 (Range > 0 && Vector2D.Distance(StartPosition, Position) >= Range);
    }
}
