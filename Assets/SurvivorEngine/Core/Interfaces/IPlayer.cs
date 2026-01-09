using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Interfaces
{
    /// <summary>
    /// 玩家接口
    /// </summary>
    public interface IPlayer
    {
        int Id { get; }
        Vector2D Position { get; set; }
        float MaxHealth { get; set; }
        float CurrentHealth { get; set; }
        float MoveSpeed { get; set; }
        int Level { get; set; }
        float Experience { get; set; }
        float ExperienceToNextLevel { get; }
        float PickupRange { get; set; }

        void TakeDamage(float damage);
        void Heal(float amount);
        void AddExperience(float amount);
        bool TryLevelUp();
        void Move(Vector2D direction, float deltaTime);
        bool IsAlive { get; }
    }
}
