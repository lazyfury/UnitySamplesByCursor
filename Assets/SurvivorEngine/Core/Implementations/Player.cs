using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Implementations
{
    /// <summary>
    /// 玩家实现
    /// </summary>
    public class Player : IPlayer
    {
        private static int _nextId = 1;

        public int Id { get; private set; }
        public Vector2D Position { get; set; }
        public float MaxHealth { get; set; }
        public float CurrentHealth { get; set; }
        public float MoveSpeed { get; set; }
        public int Level { get; set; }
        public float Experience { get; set; }
        public float ExperienceToNextLevel { get; private set; }
        public float PickupRange { get; set; }

        public bool IsAlive => CurrentHealth > 0;

        public Player()
        {
            Id = _nextId++;
            Level = 1;
            Experience = 0f;
            ExperienceToNextLevel = CalculateExperienceForLevel(2);
            PickupRange = 1.5f;
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            if (CurrentHealth < 0)
                CurrentHealth = 0;
        }

        public void Heal(float amount)
        {
            CurrentHealth += amount;
            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;
        }

        public void AddExperience(float amount)
        {
            Experience += amount;
        }

        public bool TryLevelUp()
        {
            if (Experience >= ExperienceToNextLevel)
            {
                Experience -= ExperienceToNextLevel;
                Level++;
                ExperienceToNextLevel = CalculateExperienceForLevel(Level + 1);
                return true;
            }
            return false;
        }

        public void Move(Vector2D direction, float deltaTime)
        {
            if (direction.Magnitude > 0.1f)
            {
                Vector2D normalizedDir = direction.Normalized;
                Position = Position + normalizedDir * MoveSpeed * deltaTime;
            }
        }

        private float CalculateExperienceForLevel(int level)
        {
            // 经验值需求：基础值 * (1.5 ^ (level - 1))
            return 100f * (float)System.Math.Pow(1.5f, level - 1);
        }
    }
}
