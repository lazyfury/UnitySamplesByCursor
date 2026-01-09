using System;
using System.Collections.Generic;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Interfaces
{
    /// <summary>
    /// 幸存者游戏引擎接口
    /// </summary>
    public interface ISurvivorEngine
    {
        GameState State { get; }
        IPlayer Player { get; }
        float GameTime { get; }
        int WaveNumber { get; }
        int EnemiesKilled { get; }
        float ExperienceGainMultiplier { get; set; }

        // 事件
        event Action<GameState> OnStateChanged;
        event Action<float> OnHealthChanged;
        event Action<int, float> OnLevelUp;
        event Action<IEnemy> OnEnemyKilled;
        event Action<List<UpgradeOption>> OnLevelUpAvailable;
        event Action<int> OnWaveChanged;

        // 初始化
        void Initialize(Vector2D playerStartPosition, float playerMaxHealth, float playerMoveSpeed);

        // 游戏循环
        void Update(float deltaTime);
        void SetPlayerMovement(Vector2D direction);

        // 升级系统
        void ApplyUpgrade(UpgradeOption upgrade);
        List<UpgradeOption> GetAvailableUpgrades(int count = 3);

        // 敌人管理
        List<IEnemy> GetActiveEnemies();
        void SpawnEnemy(EnemyType type, Vector2D position);

        // 武器管理
        List<IWeapon> GetActiveWeapons();
        void AddWeapon(WeaponType weaponType);
        void RemoveWeapon(WeaponType weaponType);

        // 投射物管理
        List<ProjectileData> GetActiveProjectiles();

        // 游戏控制
        void Pause();
        void Resume();
        void Restart();
    }
}
