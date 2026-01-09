using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Implementations
{
    /// <summary>
    /// 敌人实现
    /// </summary>
    public class Enemy : IEnemy
    {
        private static int _nextId = 1;

        public int Id { get; private set; }
        public EnemyType Type { get; private set; }
        public Vector2D Position { get; set; }
        public float MaxHealth { get; set; }
        public float CurrentHealth { get; set; }
        public float MoveSpeed { get; set; }
        public float Damage { get; set; }
        public float ExperienceReward { get; set; }

        public bool IsAlive => CurrentHealth > 0;

        public Enemy(EnemyType type, Vector2D position, float maxHealth, float moveSpeed, float damage, float experienceReward)
        {
            Id = _nextId++;
            Type = type;
            Position = position;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
            MoveSpeed = moveSpeed;
            Damage = damage;
            ExperienceReward = experienceReward;
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0)
                CurrentHealth = 0;
        }

        public void MoveTowards(Vector2D target, float deltaTime)
        {
            Vector2D direction = target - Position;
            float distance = direction.Magnitude;
            
            if (distance > 0.1f)
            {
                Vector2D normalizedDir = direction.Normalized;
                Position = Position + normalizedDir * MoveSpeed * deltaTime;
            }
        }
    }
}
