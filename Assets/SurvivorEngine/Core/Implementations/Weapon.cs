using System;
using System.Collections.Generic;
using System.Linq;
using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Implementations
{
    /// <summary>
    /// 武器实现
    /// </summary>
    public class Weapon : IWeapon
    {
        private static int _nextId = 1;
        private Random _random;

        public int Id { get; private set; }
        public WeaponType Type { get; private set; }
        public int Level { get; set; }
        public float Damage { get; set; }
        public float AttackSpeed { get; set; }
        public float Cooldown { get; set; }
        public float CurrentCooldown { get; set; }
        public float Range { get; set; }
        public float ProjectileSpeed { get; set; }
        public int ProjectileCount { get; set; }

        public bool IsReady => CurrentCooldown <= 0;

        public Weapon(WeaponType type, float damage, float attackSpeed, float range, float projectileSpeed, int projectileCount = 1)
        {
            Id = _nextId++;
            Type = type;
            Level = 1;
            Damage = damage;
            AttackSpeed = attackSpeed;
            Cooldown = 1f / attackSpeed;
            CurrentCooldown = 0f;
            Range = range;
            ProjectileSpeed = projectileSpeed;
            ProjectileCount = projectileCount;
            _random = new Random();
        }

        public void Update(float deltaTime)
        {
            if (CurrentCooldown > 0)
            {
                CurrentCooldown -= deltaTime;
                if (CurrentCooldown < 0)
                    CurrentCooldown = 0;
            }
        }

        public List<ProjectileData> Attack(Vector2D playerPosition, Vector2D playerDirection, List<IEnemy> enemies)
        {
            if (!IsReady)
                return new List<ProjectileData>();

            CurrentCooldown = Cooldown;
            List<ProjectileData> projectiles = new List<ProjectileData>();

            switch (Type)
            {
                case WeaponType.Whip:
                    return CreateWhipProjectile(playerPosition, playerDirection);
                
                case WeaponType.Knife:
                    return CreateKnifeProjectiles(playerPosition, enemies);
                
                case WeaponType.Fireball:
                    return CreateFireballProjectiles(playerPosition, playerDirection);
                
                case WeaponType.MagicWand:
                    return CreateMagicWandProjectiles(playerPosition, enemies);
                
                default:
                    return CreateDefaultProjectile(playerPosition, playerDirection);
            }
        }

        private List<ProjectileData> CreateWhipProjectile(Vector2D playerPosition, Vector2D playerDirection)
        {
            // 鞭子：扇形攻击
            List<ProjectileData> projectiles = new List<ProjectileData>();
            int count = ProjectileCount;
            float angleStep = 60f / count; // 60度扇形
            
            for (int i = 0; i < count; i++)
            {
                float angle = -30f + i * angleStep;
                float rad = angle * (float)Math.PI / 180f;
                Vector2D dir = new Vector2D(
                    playerDirection.X * (float)Math.Cos(rad) - playerDirection.Y * (float)Math.Sin(rad),
                    playerDirection.X * (float)Math.Sin(rad) + playerDirection.Y * (float)Math.Cos(rad)
                );
                
                projectiles.Add(new ProjectileData
                {
                    Id = _nextId++,
                    Position = playerPosition,
                    Direction = dir.Normalized,
                    Speed = ProjectileSpeed,
                    Damage = Damage,
                    Lifetime = Range / ProjectileSpeed,
                    WeaponType = Type,
                    Range = Range,
                    StartPosition = playerPosition
                });
            }
            
            return projectiles;
        }

        private List<ProjectileData> CreateKnifeProjectiles(Vector2D playerPosition, List<IEnemy> enemies)
        {
            // 飞刀：追踪最近的敌人
            List<ProjectileData> projectiles = new List<ProjectileData>();
            int count = Math.Min(ProjectileCount, enemies.Count);
            
            var sortedEnemies = enemies.OrderBy(e => Vector2D.SqrDistance(playerPosition, e.Position)).Take(count);
            
            foreach (var enemy in sortedEnemies)
            {
                Vector2D direction = (enemy.Position - playerPosition).Normalized;
                projectiles.Add(new ProjectileData
                {
                    Id = _nextId++,
                    Position = playerPosition,
                    Direction = direction,
                    Speed = ProjectileSpeed,
                    Damage = Damage,
                    Lifetime = Range / ProjectileSpeed,
                    WeaponType = Type,
                    Range = Range,
                    StartPosition = playerPosition
                });
            }
            
            return projectiles;
        }

        private List<ProjectileData> CreateFireballProjectiles(Vector2D playerPosition, Vector2D playerDirection)
        {
            // 火球：直线发射
            List<ProjectileData> projectiles = new List<ProjectileData>();
            
            for (int i = 0; i < ProjectileCount; i++)
            {
                projectiles.Add(new ProjectileData
                {
                    Id = _nextId++,
                    Position = playerPosition,
                    Direction = playerDirection.Normalized,
                    Speed = ProjectileSpeed,
                    Damage = Damage,
                    Lifetime = Range / ProjectileSpeed,
                    WeaponType = Type,
                    Range = Range,
                    StartPosition = playerPosition
                });
            }
            
            return projectiles;
        }

        private List<ProjectileData> CreateMagicWandProjectiles(Vector2D playerPosition, List<IEnemy> enemies)
        {
            // 魔法棒：随机选择敌人
            List<ProjectileData> projectiles = new List<ProjectileData>();
            
            if (enemies.Count == 0)
                return projectiles;
            
            for (int i = 0; i < ProjectileCount; i++)
            {
                var target = enemies[_random.Next(enemies.Count)];
                Vector2D direction = (target.Position - playerPosition).Normalized;
                
                projectiles.Add(new ProjectileData
                {
                    Id = _nextId++,
                    Position = playerPosition,
                    Direction = direction,
                    Speed = ProjectileSpeed,
                    Damage = Damage,
                    Lifetime = Range / ProjectileSpeed,
                    WeaponType = Type,
                    Range = Range,
                    StartPosition = playerPosition
                });
            }
            
            return projectiles;
        }

        private List<ProjectileData> CreateDefaultProjectile(Vector2D playerPosition, Vector2D playerDirection)
        {
            List<ProjectileData> projectiles = new List<ProjectileData>();
            
            projectiles.Add(new ProjectileData
            {
                Id = _nextId++,
                Position = playerPosition,
                Direction = playerDirection.Normalized,
                Speed = ProjectileSpeed,
                Damage = Damage,
                Lifetime = Range / ProjectileSpeed,
                WeaponType = Type,
                Range = Range,
                StartPosition = playerPosition
            });
            
            return projectiles;
        }

        public void Upgrade()
        {
            Level++;
            Damage *= 1.2f; // 伤害增加20%
            AttackSpeed *= 1.1f; // 攻击速度增加10%
            Cooldown = 1f / AttackSpeed;
            
            // 某些武器升级时增加投射物数量
            if (Type == WeaponType.Knife || Type == WeaponType.MagicWand)
            {
                ProjectileCount++;
            }
        }
    }
}
