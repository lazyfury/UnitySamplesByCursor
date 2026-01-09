using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Interfaces
{
    /// <summary>
    /// 敌人接口
    /// </summary>
    public interface IEnemy
    {
        int Id { get; }
        EnemyType Type { get; }
        Vector2D Position { get; set; }
        float MaxHealth { get; set; }
        float CurrentHealth { get; set; }
        float MoveSpeed { get; set; }
        float Damage { get; set; }
        float ExperienceReward { get; set; }

        void TakeDamage(float damage);
        void MoveTowards(Vector2D target, float deltaTime);
        bool IsAlive { get; }
    }
}
