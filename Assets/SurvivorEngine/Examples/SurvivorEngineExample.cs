using UnityEngine;
using SurvivorEngine.Core.Implementations;
using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Examples
{
    /// <summary>
    /// 幸存者游戏引擎使用示例
    /// </summary>
    public class SurvivorEngineExample : MonoBehaviour
    {
        private ISurvivorEngine _engine;
        private Vector2D _inputDirection;

        void Start()
        {
            // 创建引擎实例
            _engine = new SurvivorGameEngine();

            // 初始化游戏
            _engine.Initialize(
                playerStartPosition: new Vector2D(0, 0),
                playerMaxHealth: 100f,
                playerMoveSpeed: 5f
            );

            // 订阅事件
            _engine.OnStateChanged += OnStateChanged;
            _engine.OnHealthChanged += OnHealthChanged;
            _engine.OnLevelUp += OnLevelUp;
            _engine.OnEnemyKilled += OnEnemyKilled;
            _engine.OnLevelUpAvailable += OnLevelUpAvailable;
            _engine.OnWaveChanged += OnWaveChanged;

            Debug.Log("幸存者游戏引擎已初始化");
        }

        void Update()
        {
            if (_engine == null || _engine.State != GameState.Playing)
                return;

            // 获取输入（使用WASD或方向键）
            _inputDirection = Vector2D.Zero;
            
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                _inputDirection.Y += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                _inputDirection.Y -= 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                _inputDirection.X -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                _inputDirection.X += 1f;

            // 设置玩家移动方向
            _engine.SetPlayerMovement(_inputDirection);

            // 更新引擎
            _engine.Update(Time.deltaTime);
        }

        void OnStateChanged(GameState newState)
        {
            Debug.Log($"游戏状态改变: {newState}");
        }

        void OnHealthChanged(float currentHealth)
        {
            Debug.Log($"玩家生命值: {currentHealth:F1}");
        }

        void OnLevelUp(int level, float experience)
        {
            Debug.Log($"玩家升级到 {level} 级！当前经验: {experience:F1}");
        }

        void OnEnemyKilled(IEnemy enemy)
        {
            Debug.Log($"敌人被击杀: {enemy.Type}, 获得经验: {enemy.ExperienceReward}");
        }

        void OnLevelUpAvailable(System.Collections.Generic.List<UpgradeOption> upgrades)
        {
            Debug.Log("=== 升级选项 ===");
            for (int i = 0; i < upgrades.Count; i++)
            {
                var upgrade = upgrades[i];
                Debug.Log($"{i + 1}. {upgrade.Name}: {upgrade.Description}");
            }
            Debug.Log("请选择升级（在UI中实现）");
        }

        void OnWaveChanged(int waveNumber)
        {
            Debug.Log($"新波次开始: 第 {waveNumber} 波");
        }

        // 示例：应用升级（通常在UI按钮点击时调用）
        public void ApplyUpgradeExample(int upgradeIndex)
        {
            if (_engine.State != GameState.LevelUp)
                return;

            var upgrades = _engine.GetAvailableUpgrades(3);
            if (upgradeIndex >= 0 && upgradeIndex < upgrades.Count)
            {
                _engine.ApplyUpgrade(upgrades[upgradeIndex]);
                Debug.Log($"已应用升级: {upgrades[upgradeIndex].Name}");
            }
        }

        void OnDestroy()
        {
            // 取消订阅事件
            if (_engine != null)
            {
                _engine.OnStateChanged -= OnStateChanged;
                _engine.OnHealthChanged -= OnHealthChanged;
                _engine.OnLevelUp -= OnLevelUp;
                _engine.OnEnemyKilled -= OnEnemyKilled;
                _engine.OnLevelUpAvailable -= OnLevelUpAvailable;
                _engine.OnWaveChanged -= OnWaveChanged;
            }
        }

        void OnGUI()
        {
            if (_engine == null)
                return;

            var player = _engine.Player;
            if (player == null)
                return;

            // 显示游戏信息
            GUI.Label(new Rect(10, 10, 300, 20), $"生命值: {player.CurrentHealth:F1} / {player.MaxHealth:F1}");
            GUI.Label(new Rect(10, 30, 300, 20), $"等级: {player.Level}");
            GUI.Label(new Rect(10, 50, 300, 20), $"经验: {player.Experience:F1} / {player.ExperienceToNextLevel:F1}");
            GUI.Label(new Rect(10, 70, 300, 20), $"游戏时间: {_engine.GameTime:F1}秒");
            GUI.Label(new Rect(10, 90, 300, 20), $"波次: {_engine.WaveNumber}");
            GUI.Label(new Rect(10, 110, 300, 20), $"击杀数: {_engine.EnemiesKilled}");
            GUI.Label(new Rect(10, 130, 300, 20), $"敌人数量: {_engine.GetActiveEnemies().Count}");
            GUI.Label(new Rect(10, 150, 300, 20), $"武器数量: {_engine.GetActiveWeapons().Count}");

            // 显示武器信息
            int yOffset = 170;
            foreach (var weapon in _engine.GetActiveWeapons())
            {
                GUI.Label(new Rect(10, yOffset, 300, 20), 
                    $"{weapon.Type}: Lv.{weapon.Level} 伤害:{weapon.Damage:F1} 冷却:{weapon.CurrentCooldown:F2}");
                yOffset += 20;
            }

            // 升级选择提示
            if (_engine.State == GameState.LevelUp)
            {
                GUI.Box(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 150, 400, 300), "选择升级");
                var upgrades = _engine.GetAvailableUpgrades(3);
                for (int i = 0; i < upgrades.Count; i++)
                {
                    var upgrade = upgrades[i];
                    if (GUI.Button(new Rect(Screen.width / 2 - 180, Screen.height / 2 - 100 + i * 80, 360, 60), 
                        $"{upgrade.Name}\n{upgrade.Description}"))
                    {
                        _engine.ApplyUpgrade(upgrade);
                    }
                }
            }
        }
    }
}
