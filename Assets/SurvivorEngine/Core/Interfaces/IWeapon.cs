using System.Collections.Generic;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Interfaces
{
    /// <summary>
    /// 武器接口
    /// </summary>
    public interface IWeapon
    {
        int Id { get; }
        WeaponType Type { get; }
        int Level { get; set; }
        float Damage { get; set; }
        float AttackSpeed { get; set; }
        float Cooldown { get; set; }
        float CurrentCooldown { get; set; }
        float Range { get; set; }
        float ProjectileSpeed { get; set; }
        int ProjectileCount { get; set; }
        bool IsReady { get; }

        void Update(float deltaTime);
        List<ProjectileData> Attack(Vector2D playerPosition, Vector2D playerDirection, List<IEnemy> enemies);
        void Upgrade();
    }
}
