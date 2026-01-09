using System.Collections.Generic;
using UnityEngine;
using SurvivorEngine.Core.Implementations;
using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Unity
{
    /// <summary>
    /// 幸存者游戏管理器
    /// 管理Unity场景中的玩家和敌人MonoBehaviour组件与引擎的同步
    /// </summary>
    public class SurvivorGameManager : MonoBehaviour
    {
        [Header("游戏设置")]
        [SerializeField] private float playerMaxHealth = 100f;
        [SerializeField] private float playerMoveSpeed = 5f;
        [SerializeField] private Vector2 playerStartPosition = Vector2.zero;

        [Header("预制体")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private GameObject projectilePrefab;

        [Header("场景引用")]
        [SerializeField] private Transform playerContainer;
        [SerializeField] private Transform enemyContainer;
        [SerializeField] private Transform projectileContainer;

        private ISurvivorEngine _engine;
        private PlayerMonoBehaviour _playerMono;
        private Dictionary<int, EnemyMonoBehaviour> _enemyMonos = new Dictionary<int, EnemyMonoBehaviour>();
        private Dictionary<int, ProjectileMonoBehaviour> _projectileMonos = new Dictionary<int, ProjectileMonoBehaviour>();

        /// <summary>
        /// 游戏引擎实例
        /// </summary>
        public ISurvivorEngine Engine => _engine;

        void Start()
        {
            InitializeGame();
        }

        void Update()
        {
            if (_engine == null || _engine.State != GameState.Playing)
                return;

            // 处理输入
            HandleInput();

            // 更新引擎
            _engine.Update(Time.deltaTime);

            // 同步Unity对象
            SyncUnityObjects();
            
            // 同步投射物
            SyncProjectiles();
        }

        /// <summary>
        /// 初始化游戏
        /// </summary>
        private void InitializeGame()
        {
            // 创建引擎
            _engine = new SurvivorGameEngine();

            // 初始化引擎
            Vector2D startPos = new Vector2D(playerStartPosition.x, playerStartPosition.y);
            _engine.Initialize(startPos, playerMaxHealth, playerMoveSpeed);

            // 创建玩家对象
            CreatePlayer();

            // 订阅事件
            SubscribeToEvents();

            Debug.Log("游戏已初始化");
        }

        /// <summary>
        /// 创建玩家对象
        /// </summary>
        private void CreatePlayer()
        {
            GameObject playerObj;
            
            if (playerPrefab != null)
            {
                playerObj = Instantiate(playerPrefab);
            }
            else
            {
                // 如果没有预制体，创建一个简单的GameObject
                playerObj = new GameObject("Player");
                playerObj.AddComponent<SpriteRenderer>();
                playerObj.AddComponent<PlayerMonoBehaviour>();
            }

            if (playerContainer != null)
            {
                playerObj.transform.SetParent(playerContainer);
            }

            _playerMono = playerObj.GetComponent<PlayerMonoBehaviour>();
            if (_playerMono == null)
            {
                _playerMono = playerObj.AddComponent<PlayerMonoBehaviour>();
            }

            _playerMono.Player = _engine.Player;
            
            // 设置初始位置
            Vector2D pos = _engine.Player.Position;
            playerObj.transform.position = new Vector3(pos.X, pos.Y, 0);
        }

        /// <summary>
        /// 订阅引擎事件
        /// </summary>
        private void SubscribeToEvents()
        {
            _engine.OnEnemyKilled += OnEnemyKilled;
        }

        /// <summary>
        /// 处理输入
        /// </summary>
        private void HandleInput()
        {
            Vector2D input = Vector2D.Zero;

            // 键盘输入
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                input.Y += 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                input.Y -= 1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                input.X -= 1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                input.X += 1f;

            // Unity输入系统（如果使用）
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
            {
                input = new Vector2D(horizontal, vertical);
            }

            _engine.SetPlayerMovement(input);
        }

        /// <summary>
        /// 同步Unity对象与引擎数据
        /// </summary>
        private void SyncUnityObjects()
        {
            // 同步玩家
            if (_playerMono != null && _engine.Player != null)
            {
                _playerMono.SyncFromEngine();
            }

            // 同步敌人
            var activeEnemies = _engine.GetActiveEnemies();
            HashSet<int> activeEnemyIds = new HashSet<int>();

            foreach (var enemy in activeEnemies)
            {
                activeEnemyIds.Add(enemy.Id);

                if (!_enemyMonos.ContainsKey(enemy.Id))
                {
                    CreateEnemy(enemy);
                }
                else
                {
                    _enemyMonos[enemy.Id].SyncFromEngine();
                }
            }

            // 移除已死亡的敌人
            List<int> toRemove = new List<int>();
            foreach (var kvp in _enemyMonos)
            {
                if (!activeEnemyIds.Contains(kvp.Key))
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var id in toRemove)
            {
                if (_enemyMonos.ContainsKey(id))
                {
                    Destroy(_enemyMonos[id].gameObject);
                    _enemyMonos.Remove(id);
                }
            }
        }

        /// <summary>
        /// 同步投射物
        /// </summary>
        private void SyncProjectiles()
        {
            var activeProjectiles = _engine.GetActiveProjectiles();
            HashSet<int> activeProjectileIds = new HashSet<int>();

            foreach (var projectile in activeProjectiles)
            {
                activeProjectileIds.Add(projectile.Id);

                if (!_projectileMonos.ContainsKey(projectile.Id))
                {
                    CreateProjectile(projectile);
                }
                else
                {
                    // 更新投射物数据引用（因为引擎会更新数据）
                    _projectileMonos[projectile.Id].Projectile = projectile;
                    _projectileMonos[projectile.Id].SyncFromData();
                }
            }

            // 移除已过期或命中的投射物
            List<int> toRemove = new List<int>();
            foreach (var kvp in _projectileMonos)
            {
                if (!activeProjectileIds.Contains(kvp.Key))
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var id in toRemove)
            {
                if (_projectileMonos.ContainsKey(id))
                {
                    Destroy(_projectileMonos[id].gameObject);
                    _projectileMonos.Remove(id);
                }
            }
        }

        /// <summary>
        /// 创建投射物对象
        /// </summary>
        private void CreateProjectile(ProjectileData projectile)
        {
            GameObject projectileObj;

            if (projectilePrefab != null)
            {
                projectileObj = Instantiate(projectilePrefab);
            }
            else
            {
                // 如果没有预制体，创建一个简单的GameObject
                projectileObj = new GameObject($"Projectile_{projectile.WeaponType}_{projectile.Id}");
                projectileObj.AddComponent<SpriteRenderer>();
                projectileObj.AddComponent<ProjectileMonoBehaviour>();
            }

            if (projectileContainer != null)
            {
                projectileObj.transform.SetParent(projectileContainer);
            }

            ProjectileMonoBehaviour projectileMono = projectileObj.GetComponent<ProjectileMonoBehaviour>();
            if (projectileMono == null)
            {
                projectileMono = projectileObj.AddComponent<ProjectileMonoBehaviour>();
            }

            projectileMono.Projectile = projectile;
            _projectileMonos[projectile.Id] = projectileMono;

            // 设置初始位置
            Vector2D pos = projectile.Position;
            projectileObj.transform.position = new Vector3(pos.X, pos.Y, 0);
        }

        /// <summary>
        /// 创建敌人对象
        /// </summary>
        private void CreateEnemy(IEnemy enemy)
        {
            GameObject enemyObj;

            if (enemyPrefab != null)
            {
                enemyObj = Instantiate(enemyPrefab);
            }
            else
            {
                // 如果没有预制体，创建一个简单的GameObject
                enemyObj = new GameObject($"Enemy_{enemy.Type}_{enemy.Id}");
                enemyObj.AddComponent<SpriteRenderer>();
                enemyObj.AddComponent<EnemyMonoBehaviour>();
            }

            if (enemyContainer != null)
            {
                enemyObj.transform.SetParent(enemyContainer);
            }

            EnemyMonoBehaviour enemyMono = enemyObj.GetComponent<EnemyMonoBehaviour>();
            if (enemyMono == null)
            {
                enemyMono = enemyObj.AddComponent<EnemyMonoBehaviour>();
            }

            enemyMono.Enemy = enemy;
            _enemyMonos[enemy.Id] = enemyMono;

            // 设置初始位置
            Vector2D pos = enemy.Position;
            enemyObj.transform.position = new Vector3(pos.X, pos.Y, 0);
        }

        /// <summary>
        /// 敌人被击杀时的回调
        /// </summary>
        private void OnEnemyKilled(IEnemy enemy)
        {
            // 可以在这里添加死亡特效、音效等
            if (_enemyMonos.ContainsKey(enemy.Id))
            {
                // 延迟销毁，以便播放死亡动画
                StartCoroutine(DestroyEnemyDelayed(enemy.Id, 0.1f));
            }
        }

        /// <summary>
        /// 延迟销毁敌人
        /// </summary>
        private System.Collections.IEnumerator DestroyEnemyDelayed(int enemyId, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (_enemyMonos.ContainsKey(enemyId))
            {
                Destroy(_enemyMonos[enemyId].gameObject);
                _enemyMonos.Remove(enemyId);
            }
        }

        void OnDestroy()
        {
            if (_engine != null)
            {
                _engine.OnEnemyKilled -= OnEnemyKilled;
            }
        }
    }
}
