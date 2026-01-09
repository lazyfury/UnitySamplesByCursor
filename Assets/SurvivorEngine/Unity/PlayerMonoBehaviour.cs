using UnityEngine;
using SurvivorEngine.Core.Interfaces;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Unity
{
    /// <summary>
    /// 玩家MonoBehaviour组件
    /// 用于在Unity场景中表示玩家实体
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerMonoBehaviour : MonoBehaviour
    {
        [Header("玩家设置")]
        [SerializeField] private Color playerColor = Color.blue;
        [SerializeField] private float spriteSize = 0.5f;
        
        [Header("调试")]
        [SerializeField] private bool showHealthBar = true;
        [SerializeField] private bool showPickupRange = true;

        private IPlayer _player;
        private SpriteRenderer _spriteRenderer;
        private Vector2D _lastPosition;

        /// <summary>
        /// 绑定的玩家数据
        /// </summary>
        public IPlayer Player
        {
            get => _player;
            set
            {
                _player = value;
                if (_player != null)
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
            if (_player != null)
            {
                SyncFromEngine();
            }
        }

        void Update()
        {
            if (_player == null)
                return;

            // 同步位置（从引擎到Unity）
            SyncPositionFromEngine();
        }

        void LateUpdate()
        {
            // 同步位置到引擎（从Unity到引擎）
            if (_player != null && transform.hasChanged)
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
            if (_player == null)
                return;

            SyncPositionFromEngine();
            UpdateVisuals();
        }

        /// <summary>
        /// 同步位置到引擎
        /// </summary>
        public void SyncPositionToEngine()
        {
            if (_player == null)
                return;

            Vector3 pos = transform.position;
            _player.Position = new Vector2D(pos.x, pos.y);
        }

        /// <summary>
        /// 从引擎同步位置
        /// </summary>
        private void SyncPositionFromEngine()
        {
            if (_player == null)
                return;

            Vector2D enginePos = _player.Position;
            Vector3 unityPos = new Vector3(enginePos.X, enginePos.Y, transform.position.z);
            
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
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = playerColor;
            }
        }

        /// <summary>
        /// 设置精灵
        /// </summary>
        private void SetupSprite()
        {
            if (_spriteRenderer == null)
                return;

            // 如果没有精灵，创建一个简单的方形
            if (_spriteRenderer.sprite == null)
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();
                
                Sprite sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, 1, 1),
                    new Vector2(0.5f, 0.5f),
                    100f
                );
                _spriteRenderer.sprite = sprite;
            }

            transform.localScale = Vector3.one * spriteSize;
        }

        void OnDrawGizmos()
        {
            if (_player == null)
                return;

            // 绘制拾取范围
            if (showPickupRange)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, _player.PickupRange);
            }

            // 绘制生命值条（在Scene视图中）
            if (showHealthBar && _player.MaxHealth > 0)
            {
                float healthPercent = _player.CurrentHealth / _player.MaxHealth;
                Vector3 barPos = transform.position + Vector3.up * 0.8f;
                
                // 背景
                Gizmos.color = Color.red;
                Gizmos.DrawLine(barPos + Vector3.left * 0.5f, barPos + Vector3.right * 0.5f);
                
                // 生命值
                Gizmos.color = Color.green;
                float barLength = healthPercent * 1f;
                Gizmos.DrawLine(barPos + Vector3.left * 0.5f, barPos + Vector3.left * 0.5f + Vector3.right * barLength);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (_player == null)
                return;

            // 显示玩家信息
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}
