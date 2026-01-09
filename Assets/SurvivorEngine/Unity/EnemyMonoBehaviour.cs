using UnityEngine;
using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Unity
{
    /// <summary>
    /// 敌人MonoBehaviour组件
    /// 用于在Unity场景中表示敌人实体
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyMonoBehaviour : MonoBehaviour
    {
        [Header("敌人设置")]
        [SerializeField] private Color[] enemyColors = new Color[]
        {
            Color.red,      // Bat
            Color.gray,     // Skeleton
            Color.cyan,     // Ghost
            Color.green,    // Zombie
            Color.magenta   // Boss
        };
        [SerializeField] private float spriteSize = 0.4f;
        
        [Header("调试")]
        [SerializeField] private bool showHealthBar = true;
        [SerializeField] private bool showDirection = false;

        private IEnemy _enemy;
        private SpriteRenderer _spriteRenderer;
        private Vector2D _lastPosition;
        private Vector2D _velocity;

        /// <summary>
        /// 绑定的敌人数据
        /// </summary>
        public IEnemy Enemy
        {
            get => _enemy;
            set
            {
                _enemy = value;
                if (_enemy != null)
                {
                    SyncFromEngine();
                }
            }
        }

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            SetupSprite();
        }

        void Start()
        {
            if (_enemy != null)
            {
                SyncFromEngine();
            }
        }

        void Update()
        {
            if (_enemy == null)
                return;

            // 同步位置（从引擎到Unity）
            SyncPositionFromEngine();
            
            // 更新视觉效果
            UpdateVisuals();
        }

        void LateUpdate()
        {
            // 同步位置到引擎（从Unity到引擎，如果需要）
            if (_enemy != null && transform.hasChanged)
            {
                SyncPositionToEngine();
                transform.hasChanged = false;
            }
        }

        /// <summary>
        /// 从引擎同步所有数据
        /// </summary>
        public void SyncFromEngine()
        {
            if (_enemy == null)
                return;

            SyncPositionFromEngine();
            UpdateVisuals();
        }

        /// <summary>
        /// 同步位置到引擎
        /// </summary>
        public void SyncPositionToEngine()
        {
            if (_enemy == null)
                return;

            Vector3 pos = transform.position;
            _enemy.Position = new Vector2D(pos.x, pos.y);
        }

        /// <summary>
        /// 从引擎同步位置
        /// </summary>
        private void SyncPositionFromEngine()
        {
            if (_enemy == null)
                return;

            Vector2D enginePos = _enemy.Position;
            Vector3 unityPos = new Vector3(enginePos.X, enginePos.Y, transform.position.z);
            
            // 计算速度（用于方向指示）
            if (_lastPosition.Magnitude > 0.01f)
            {
                _velocity = enginePos - _lastPosition;
            }
            _lastPosition = enginePos;
            
            if (Vector3.Distance(transform.position, unityPos) > 0.01f)
            {
                transform.position = unityPos;
            }
        }

        /// <summary>
        /// 更新视觉效果
        /// </summary>
        private void UpdateVisuals()
        {
            if (_spriteRenderer == null || _enemy == null)
                return;

            // 根据敌人类型设置颜色
            int typeIndex = (int)_enemy.Type;
            if (typeIndex >= 0 && typeIndex < enemyColors.Length)
            {
                _spriteRenderer.color = enemyColors[typeIndex];
            }
            else
            {
                _spriteRenderer.color = Color.red;
            }

            // 根据生命值调整透明度（低生命值时变透明）
            if (_enemy.MaxHealth > 0)
            {
                float healthPercent = _enemy.CurrentHealth / _enemy.MaxHealth;
                Color color = _spriteRenderer.color;
                color.a = Mathf.Lerp(0.5f, 1f, healthPercent);
                _spriteRenderer.color = color;
            }
        }

        /// <summary>
        /// 设置精灵
        /// </summary>
        private void SetupSprite()
        {
            if (_spriteRenderer == null)
                return;

            // 如果没有精灵，创建一个简单的圆形
            if (_spriteRenderer.sprite == null)
            {
                int size = 32;
                Texture2D texture = new Texture2D(size, size);
                
                Vector2 center = new Vector2(size / 2f, size / 2f);
                float radius = size / 2f - 2f;
                
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float dist = Vector2.Distance(new Vector2(x, y), center);
                        if (dist <= radius)
                        {
                            texture.SetPixel(x, y, Color.white);
                        }
                        else
                        {
                            texture.SetPixel(x, y, Color.clear);
                        }
                    }
                }
                
                texture.Apply();
                
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, size, size),
                    new Vector2(0.5f, 0.5f),
                    100f
                );
                _spriteRenderer.sprite = sprite;
            }

            transform.localScale = Vector3.one * spriteSize;
        }

        void OnDrawGizmos()
        {
            if (_enemy == null)
                return;

            // 绘制生命值条
            if (showHealthBar && _enemy.MaxHealth > 0)
            {
                float healthPercent = _enemy.CurrentHealth / _enemy.MaxHealth;
                Vector3 barPos = transform.position + Vector3.up * 0.6f;
                
                // 背景
                Gizmos.color = Color.red;
                Gizmos.DrawLine(barPos + Vector3.left * 0.4f, barPos + Vector3.right * 0.4f);
                
                // 生命值
                Gizmos.color = Color.green;
                float barLength = healthPercent * 0.8f;
                Gizmos.DrawLine(barPos + Vector3.left * 0.4f, barPos + Vector3.left * 0.4f + Vector3.right * barLength);
            }

            // 绘制移动方向
            if (showDirection && _velocity.Magnitude > 0.1f)
            {
                Gizmos.color = Color.yellow;
                Vector3 direction = new Vector3(_velocity.X, _velocity.Y, 0).normalized * 0.5f;
                Gizmos.DrawRay(transform.position, direction);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (_enemy == null)
                return;

            // 显示敌人信息
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.25f);
        }
    }
}
