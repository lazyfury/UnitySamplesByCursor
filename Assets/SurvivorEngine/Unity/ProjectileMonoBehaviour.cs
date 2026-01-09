using UnityEngine;
using SurvivorEngine.Core.Types;

namespace SurvivorEngine.Unity
{
    /// <summary>
    /// 投射物MonoBehaviour组件
    /// 用于在Unity场景中显示投射物
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ProjectileMonoBehaviour : MonoBehaviour
    {
        [Header("投射物设置")]
        [SerializeField] private Color[] weaponColors = new Color[]
        {
            Color.white,    // None
            Color.yellow,   // Whip
            Color.cyan,     // Knife
            Color.red,      // Fireball
            Color.magenta,  // MagicWand
            Color.blue,     // Cross
            Color.green,    // Axe
            Color.gray,     // Garlic
            Color.white,    // Bible
            Color.cyan,     // Runetracer
            Color.yellow    // KingBible
        };
        [SerializeField] private float spriteSize = 0.2f;

        private ProjectileData _projectile;
        private SpriteRenderer _spriteRenderer;

        /// <summary>
        /// 绑定的投射物数据
        /// </summary>
        public ProjectileData Projectile
        {
            get => _projectile;
            set
            {
                _projectile = value;
                if (_projectile != null)
                {
                    SyncFromData();
                }
            }
        }

        void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            SetupSprite();
        }

        void Update()
        {
            if (_projectile == null)
                return;

            // 同步位置
            SyncPositionFromData();
        }

        /// <summary>
        /// 从数据同步
        /// </summary>
        public void SyncFromData()
        {
            if (_projectile == null)
                return;

            SyncPositionFromData();
            UpdateVisuals();
        }

        /// <summary>
        /// 同步位置
        /// </summary>
        private void SyncPositionFromData()
        {
            if (_projectile == null)
                return;

            Vector2D pos = _projectile.Position;
            transform.position = new Vector3(pos.X, pos.Y, transform.position.z);

            // 根据方向旋转
            if (_projectile.Direction.Magnitude > 0.1f)
            {
                float angle = Mathf.Atan2(_projectile.Direction.Y, _projectile.Direction.X) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        /// <summary>
        /// 更新视觉效果
        /// </summary>
        private void UpdateVisuals()
        {
            if (_spriteRenderer == null || _projectile == null)
                return;

            // 根据武器类型设置颜色
            int typeIndex = (int)_projectile.WeaponType;
            if (typeIndex >= 0 && typeIndex < weaponColors.Length)
            {
                _spriteRenderer.color = weaponColors[typeIndex];
            }
            else
            {
                _spriteRenderer.color = Color.white;
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
                int size = 16;
                Texture2D texture = new Texture2D(size, size);
                
                Vector2 center = new Vector2(size / 2f, size / 2f);
                float radius = size / 2f - 1f;
                
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
    }
}
