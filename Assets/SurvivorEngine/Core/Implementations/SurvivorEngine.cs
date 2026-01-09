using System;
using System.Collections.Generic;
using System.Linq;
using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Systems;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Core.Implementations
{
    /// <summary>
    /// 幸存者游戏引擎实现
    /// </summary>
    public class SurvivorGameEngine : ISurvivorEngine
    {
        private IPlayer _player;
        private List<IEnemy> _enemies;
        private List<IWeapon> _weapons;
        private ProjectileSystem _projectileSystem;
        private EnemySpawner _enemySpawner;
        private UpgradeSystem _upgradeSystem;
        
        private GameState _state;
        private float _gameTime;
        private int _waveNumber;
        private int _enemiesKilled;
        private Vector2D _playerMovementDirection;
        private Vector2D _lastPlayerDirection;
        private float _experienceGainMultiplier;

        // 事件
        public event Action<GameState> OnStateChanged;
        public event Action<float> OnHealthChanged;
        public event Action<int, float> OnLevelUp;
        public event Action<IEnemy> OnEnemyKilled;
        public event Action<List<UpgradeOption>> OnLevelUpAvailable;
        public event Action<int> OnWaveChanged;

        // 属性
        public GameState State
        {
            get => _state;
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged?.Invoke(_state);
                }
            }
        }

        public IPlayer Player => _player;
        public float GameTime => _gameTime;
        public int WaveNumber => _waveNumber;
        public int EnemiesKilled => _enemiesKilled;
        public float ExperienceGainMultiplier
        {
            get => _experienceGainMultiplier;
            set => _experienceGainMultiplier = Math.Max(0.1f, value);
        }

        public SurvivorGameEngine()
        {
            _enemies = new List<IEnemy>();
            _weapons = new List<IWeapon>();
            _projectileSystem = new ProjectileSystem();
            _upgradeSystem = new UpgradeSystem();
            _state = GameState.NotInitialized;
            _gameTime = 0f;
            _waveNumber = 1;
            _enemiesKilled = 0;
            _playerMovementDirection = Vector2D.Zero;
            _lastPlayerDirection = new Vector2D(0, 1); // 默认向上
            _experienceGainMultiplier = 1f;
        }

        public void Initialize(Vector2D playerStartPosition, float playerMaxHealth, float playerMoveSpeed)
        {
            _player = new Player
            {
                Position = playerStartPosition,
                MaxHealth = playerMaxHealth,
                CurrentHealth = playerMaxHealth,
                MoveSpeed = playerMoveSpeed
            };

            _enemySpawner = new EnemySpawner(
                new Vector2D(-20, -20),
                new Vector2D(20, 20)
            );

            // 初始武器
            AddWeapon(WeaponType.Whip);

            State = GameState.Playing;
        }

        public void Update(float deltaTime)
        {
            if (State != GameState.Playing)
                return;

            _gameTime += deltaTime;

            // 更新玩家
            if (_player != null && _player.IsAlive)
            {
                _player.Move(_playerMovementDirection, deltaTime);
                
                // 更新玩家方向（用于武器攻击）
                if (_playerMovementDirection.Magnitude > 0.1f)
                {
                    _lastPlayerDirection = _playerMovementDirection.Normalized;
                }

                // 检查升级
                if (_player.TryLevelUp())
                {
                    State = GameState.LevelUp;
                    var upgrades = _upgradeSystem.GenerateUpgradeOptions(3, _player, _weapons);
                    OnLevelUp?.Invoke(_player.Level, _player.Experience);
                    OnLevelUpAvailable?.Invoke(upgrades);
                    return;
                }
            }
            else
            {
                State = GameState.GameOver;
                return;
            }

            // 更新武器
            foreach (var weapon in _weapons)
            {
                weapon.Update(deltaTime);
                
                if (weapon.IsReady)
                {
                    var projectiles = weapon.Attack(_player.Position, _lastPlayerDirection, _enemies.Where(e => e.IsAlive).ToList());
                    _projectileSystem.AddProjectiles(projectiles);
                }
            }

            // 更新投射物
            _projectileSystem.Update(deltaTime, _enemies.Where(e => e.IsAlive).ToList());

            // 更新敌人
            foreach (var enemy in _enemies.ToList())
            {
                if (!enemy.IsAlive)
                    continue;

                enemy.MoveTowards(_player.Position, deltaTime);

                // 检查敌人碰撞玩家
                float distance = Vector2D.Distance(enemy.Position, _player.Position);
                if (distance < 0.5f)
                {
                    _player.TakeDamage(enemy.Damage * deltaTime); // 持续伤害
                    OnHealthChanged?.Invoke(_player.CurrentHealth);
                    
                    if (!_player.IsAlive)
                    {
                        State = GameState.GameOver;
                        return;
                    }
                }
            }

            // 移除死亡的敌人
            var deadEnemies = _enemies.Where(e => !e.IsAlive).ToList();
            foreach (var enemy in deadEnemies)
            {
                _player.AddExperience(enemy.ExperienceReward * _experienceGainMultiplier);
                _enemiesKilled++;
                OnEnemyKilled?.Invoke(enemy);
            }
            _enemies.RemoveAll(e => !e.IsAlive);

            // 更新敌人生成
            _enemySpawner.Update(deltaTime, _player.Position, _gameTime, _waveNumber);
            
            if (_enemySpawner.ShouldSpawn())
            {
                var enemyType = _enemySpawner.GetRandomEnemyType(_waveNumber);
                var spawnPos = _enemySpawner.GetSpawnPosition();
                SpawnEnemy(enemyType, spawnPos);
            }

            // 检查波次完成
            if (_enemySpawner.IsWaveComplete(_enemies.Count))
            {
                _waveNumber++;
                _enemySpawner.ResetWave();
                OnWaveChanged?.Invoke(_waveNumber);
            }
        }

        public void SetPlayerMovement(Vector2D direction)
        {
            _playerMovementDirection = direction;
        }

        public void ApplyUpgrade(UpgradeOption upgrade)
        {
            _upgradeSystem.ApplyUpgrade(upgrade, _player, _weapons);

            if (upgrade.Type == UpgradeType.NewWeapon && upgrade.WeaponType.HasValue)
            {
                AddWeapon(upgrade.WeaponType.Value);
            }
            else if (upgrade.Type == UpgradeType.ExperienceGain)
            {
                _experienceGainMultiplier *= (1f + upgrade.Value);
            }

            State = GameState.Playing;
        }

        public List<UpgradeOption> GetAvailableUpgrades(int count = 3)
        {
            return _upgradeSystem.GenerateUpgradeOptions(count, _player, _weapons);
        }

        public List<IEnemy> GetActiveEnemies()
        {
            return _enemies.Where(e => e.IsAlive).ToList();
        }

        public void SpawnEnemy(EnemyType type, Vector2D position)
        {
            float maxHealth = 50f;
            float moveSpeed = 2f;
            float damage = 10f;
            float experienceReward = 10f;

            // 根据类型和波次调整属性
            switch (type)
            {
                case EnemyType.Bat:
                    maxHealth = 30f + _waveNumber * 5f;
                    moveSpeed = 3f;
                    damage = 5f + _waveNumber * 1f;
                    experienceReward = 5f + _waveNumber * 2f;
                    break;
                case EnemyType.Skeleton:
                    maxHealth = 60f + _waveNumber * 8f;
                    moveSpeed = 1.5f;
                    damage = 10f + _waveNumber * 2f;
                    experienceReward = 10f + _waveNumber * 3f;
                    break;
                case EnemyType.Ghost:
                    maxHealth = 40f + _waveNumber * 6f;
                    moveSpeed = 2.5f;
                    damage = 8f + _waveNumber * 1.5f;
                    experienceReward = 8f + _waveNumber * 2.5f;
                    break;
                case EnemyType.Zombie:
                    maxHealth = 80f + _waveNumber * 10f;
                    moveSpeed = 1f;
                    damage = 15f + _waveNumber * 3f;
                    experienceReward = 15f + _waveNumber * 4f;
                    break;
            }

            var enemy = new Enemy(type, position, maxHealth, moveSpeed, damage, experienceReward);
            _enemies.Add(enemy);
        }

        public List<IWeapon> GetActiveWeapons()
        {
            return new List<IWeapon>(_weapons);
        }

        public void AddWeapon(WeaponType weaponType)
        {
            if (_weapons.Any(w => w.Type == weaponType))
                return;

            IWeapon weapon = null;
            switch (weaponType)
            {
                case WeaponType.Whip:
                    weapon = new Weapon(WeaponType.Whip, 20f, 1f, 3f, 10f, 1);
                    break;
                case WeaponType.Knife:
                    weapon = new Weapon(WeaponType.Knife, 15f, 2f, 8f, 12f, 1);
                    break;
                case WeaponType.Fireball:
                    weapon = new Weapon(WeaponType.Fireball, 25f, 0.8f, 10f, 8f, 1);
                    break;
                case WeaponType.MagicWand:
                    weapon = new Weapon(WeaponType.MagicWand, 18f, 1.5f, 8f, 10f, 1);
                    break;
            }

            if (weapon != null)
            {
                _weapons.Add(weapon);
            }
        }

        public void RemoveWeapon(WeaponType weaponType)
        {
            _weapons.RemoveAll(w => w.Type == weaponType);
        }

        public List<ProjectileData> GetActiveProjectiles()
        {
            return _projectileSystem.GetActiveProjectiles();
        }

        public void Pause()
        {
            if (State == GameState.Playing)
            {
                State = GameState.Paused;
            }
        }

        public void Resume()
        {
            if (State == GameState.Paused)
            {
                State = GameState.Playing;
            }
        }

        public void Restart()
        {
            _enemies.Clear();
            _weapons.Clear();
            _projectileSystem.Clear();
            _gameTime = 0f;
            _waveNumber = 1;
            _enemiesKilled = 0;
            _experienceGainMultiplier = 1f;

            if (_player != null)
            {
                _player.CurrentHealth = _player.MaxHealth;
                _player.Level = 1;
                _player.Experience = 0f;
            }

            _enemySpawner?.ResetWave();
            State = GameState.Playing;
        }
    }
}
