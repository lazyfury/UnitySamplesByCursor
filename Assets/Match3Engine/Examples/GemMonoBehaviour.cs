using UnityEngine;
using UnityEngine.Events;
using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Types;

namespace Match3Engine.Examples
{
    /// <summary>
    /// Gem的MonoBehaviour组件，用于Unity中的Gem可视化表示和交互
    /// </summary>
    public class GemMonoBehaviour : MonoBehaviour
    {
        [Header("Gem Data")]
        private IGem _gem;
        
        [Header("Visual Components")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private UnityEngine.UI.Image _image; // 如果使用UI系统
        
        [Header("Selection")]
        [SerializeField] private bool _isSelected = false;
        [SerializeField] private Color _selectedColor = Color.yellow;
        [SerializeField] private float _selectedScale = 1.2f;
        
        [Header("Animation")]
        [SerializeField] private float _swapAnimationDuration = 0.3f;
        [SerializeField] private float _eliminateAnimationDuration = 0.5f;
        [SerializeField] private AnimationCurve _swapAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
        // 事件
        public UnityEvent<GemMonoBehaviour> OnGemClicked;
        public UnityEvent<GemMonoBehaviour> OnGemSelected;
        public UnityEvent<GemMonoBehaviour> OnGemDeselected;
        
        private Vector3 _targetPosition;
        private bool _isAnimating = false;
        private Color _originalColor;
        private Vector3 _originalScale;
        
        /// <summary>
        /// 关联的Gem数据
        /// </summary>
        public IGem Gem
        {
            get => _gem;
            set
            {
                _gem = value;
                UpdateVisual();
            }
        }
        
        /// <summary>
        /// 是否为选中状态
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    UpdateSelectionVisual();
                    
                    if (_isSelected)
                    {
                        OnGemSelected?.Invoke(this);
                    }
                    else
                    {
                        OnGemDeselected?.Invoke(this);
                    }
                }
            }
        }
        
        /// <summary>
        /// 宝石位置（从Gem获取）
        /// </summary>
        public BoardPosition Position => _gem != null ? _gem.Position : new BoardPosition(0, 0);
        
        /// <summary>
        /// 宝石类型（从Gem获取）
        /// </summary>
        public GemType Type => _gem != null ? _gem.Type : GemType.None;
        
        private void Awake()
        {
            // 自动查找组件
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            if (_image == null)
            {
                _image = GetComponent<UnityEngine.UI.Image>();
            }
            
            // 保存原始值
            if (_spriteRenderer != null)
            {
                _originalColor = _spriteRenderer.color;
            }
            else if (_image != null)
            {
                _originalColor = _image.color;
            }
            
            _originalScale = transform.localScale;
            _targetPosition = transform.position;
        }
        
        private void Update()
        {
            // 平滑移动到目标位置（如果正在动画）
            if (_isAnimating && Vector3.Distance(transform.position, _targetPosition) > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * 10f);
            }
            else if (_isAnimating)
            {
                transform.position = _targetPosition;
                _isAnimating = false;
            }
        }
        
        /// <summary>
        /// 初始化Gem数据
        /// </summary>
        public void Initialize(IGem gem)
        {
            _gem = gem;
            UpdateVisual();
        }
        
        /// <summary>
        /// 更新视觉表现（颜色、材质等）
        /// </summary>
        public void UpdateVisual()
        {
            if (_gem == null)
                return;
            
            Color gemColor = GetGemColor(_gem.Type);
            
            // 更新SpriteRenderer
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = gemColor;
            }
            
            // 更新UI Image
            if (_image != null)
            {
                _image.color = gemColor;
            }
            
            // 更新名称（方便调试）
            gameObject.name = $"Gem_{_gem.Type}_{_gem.Position}";
        }
        
        /// <summary>
        /// 更新选中状态的视觉效果
        /// </summary>
        private void UpdateSelectionVisual()
        {
            if (_isSelected)
            {
                // 选中效果：高亮和放大
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.color = _selectedColor;
                }
                
                if (_image != null)
                {
                    _image.color = _selectedColor;
                }
                
                transform.localScale = _originalScale * _selectedScale;
            }
            else
            {
                // 恢复正常状态
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.color = _originalColor;
                }
                
                if (_image != null)
                {
                    _image.color = _originalColor;
                }
                
                transform.localScale = _originalScale;
            }
        }
        
        /// <summary>
        /// 根据宝石类型获取颜色
        /// </summary>
        private Color GetGemColor(GemType gemType)
        {
            return gemType switch
            {
                GemType.Red => Color.red,
                GemType.Blue => Color.blue,
                GemType.Green => Color.green,
                GemType.Yellow => Color.yellow,
                GemType.Purple => new Color(0.5f, 0f, 0.5f, 1f),
                GemType.Orange => new Color(1f, 0.5f, 0f, 1f),
                GemType.Special => Color.magenta,
                _ => Color.white
            };
        }
        
        /// <summary>
        /// 设置目标位置（用于移动动画）
        /// </summary>
        public void SetTargetPosition(Vector3 position)
        {
            _targetPosition = position;
            _isAnimating = true;
        }
        
        /// <summary>
        /// 立即设置位置（无动画）
        /// </summary>
        public void SetPosition(Vector3 position)
        {
            _targetPosition = position;
            transform.position = position;
            _isAnimating = false;
        }
        
        /// <summary>
        /// 执行交换动画
        /// </summary>
        public void AnimateSwap(Vector3 targetPosition, System.Action onComplete = null)
        {
            StartCoroutine(AnimateSwapCoroutine(targetPosition, onComplete));
        }
        
        private System.Collections.IEnumerator AnimateSwapCoroutine(Vector3 targetPosition, System.Action onComplete)
        {
            Vector3 startPosition = transform.position;
            float elapsed = 0f;
            
            while (elapsed < _swapAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _swapAnimationDuration;
                float curveValue = _swapAnimationCurve.Evaluate(t);
                
                transform.position = Vector3.Lerp(startPosition, targetPosition, curveValue);
                yield return null;
            }
            
            transform.position = targetPosition;
            _targetPosition = targetPosition;
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// 执行消除动画
        /// </summary>
        public void AnimateEliminate(System.Action onComplete = null)
        {
            StartCoroutine(AnimateEliminateCoroutine(onComplete));
        }
        
        private System.Collections.IEnumerator AnimateEliminateCoroutine(System.Action onComplete)
        {
            Vector3 startScale = transform.localScale;
            Color startColor = _spriteRenderer != null ? _spriteRenderer.color : _image.color;
            
            float elapsed = 0f;
            
            while (elapsed < _eliminateAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _eliminateAnimationDuration;
                
                // 缩放动画（缩小）
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                
                // 淡出动画
                Color currentColor = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0f), t);
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.color = currentColor;
                }
                if (_image != null)
                {
                    _image.color = currentColor;
                }
                
                yield return null;
            }
            
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// 执行出现动画（新宝石生成）
        /// </summary>
        public void AnimateAppear(System.Action onComplete = null)
        {
            StartCoroutine(AnimateAppearCoroutine(onComplete));
        }
        
        private System.Collections.IEnumerator AnimateAppearCoroutine(System.Action onComplete)
        {
            transform.localScale = Vector3.zero;
            Color color = _spriteRenderer != null ? _spriteRenderer.color : _image.color;
            color.a = 0f;
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = color;
            }
            if (_image != null)
            {
                _image.color = color;
            }
            
            float duration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                transform.localScale = Vector3.Lerp(Vector3.zero, _originalScale, t);
                color.a = t;
                
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.color = color;
                }
                if (_image != null)
                {
                    _image.color = color;
                }
                
                yield return null;
            }
            
            transform.localScale = _originalScale;
            color.a = 1f;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = color;
            }
            if (_image != null)
            {
                _image.color = color;
            }
            
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// 处理点击事件
        /// </summary>
        private void OnMouseDown()
        {
            HandleClick();
        }
        
        /// <summary>
        /// UI点击事件（如果使用UI系统）
        /// </summary>
        public void OnPointerClick()
        {
            HandleClick();
        }
        
        private void HandleClick()
        {
            if (_gem == null || _gem.Type == GemType.None)
                return;
            
            OnGemClicked?.Invoke(this);
        }
        
        /// <summary>
        /// 重置为初始状态
        /// </summary>
        public void ResetVisual()
        {
            _isSelected = false;
            transform.localScale = _originalScale;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor;
            }
            if (_image != null)
            {
                _image.color = _originalColor;
            }
        }
        
        /// <summary>
        /// 设置高亮状态（匹配时使用）
        /// </summary>
        public void SetHighlight(bool highlight, Color? highlightColor = null)
        {
            Color color = highlightColor ?? Color.white;
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = highlight ? color : _originalColor;
            }
            
            if (_image != null)
            {
                _image.color = highlight ? color : _originalColor;
            }
        }
    }
}
