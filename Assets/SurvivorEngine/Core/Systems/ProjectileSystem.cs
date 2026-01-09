using System.Collections.Generic;
using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Systems
{
    /// <summary>
    /// 投射物系统
    /// </summary>
    public class ProjectileSystem
    {
        private List<ProjectileData> _activeProjectiles;
        private int _nextProjectileId;

        public ProjectileSystem()
        {
            _activeProjectiles = new List<ProjectileData>();
            _nextProjectileId = 1;
        }

        public void Update(float deltaTime, List<IEnemy> enemies)
        {
            for (int i = _activeProjectiles.Count - 1; i >= 0; i--)
            {
                var projectile = _activeProjectiles[i];
                
                // 更新位置
                projectile.Position = projectile.Position + projectile.Direction * projectile.Speed * deltaTime;
                projectile.CurrentLifetime += deltaTime;
                
                // 检查碰撞
                bool hit = false;
                foreach (var enemy in enemies)
                {
                    if (!enemy.IsAlive)
                        continue;
                    
                    float distance = Vector2D.Distance(projectile.Position, enemy.Position);
                    if (distance < 0.5f) // 碰撞半径
                    {
                        enemy.TakeDamage(projectile.Damage);
                        hit = true;
                        break;
                    }
                }
                
                // 移除过期或命中的投射物
                if (projectile.IsExpired || hit)
                {
                    _activeProjectiles.RemoveAt(i);
                }
            }
        }

        public void AddProjectiles(List<ProjectileData> projectiles)
        {
            foreach (var projectile in projectiles)
            {
                projectile.Id = _nextProjectileId++;
                _activeProjectiles.Add(projectile);
            }
        }

        public List<ProjectileData> GetActiveProjectiles()
        {
            return new List<ProjectileData>(_activeProjectiles);
        }

        public void Clear()
        {
            _activeProjectiles.Clear();
        }
    }
}
