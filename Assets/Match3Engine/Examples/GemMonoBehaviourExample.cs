using UnityEngine;
using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Types;
using Match3Engine.Core.Implementations;

namespace Match3Engine.Examples
{
    /// <summary>
    /// GemMonoBehaviour使用示例
    /// 演示如何创建和使用Gem的MonoBehaviour组件
    /// </summary>
    public class GemMonoBehaviourExample : MonoBehaviour
    {
        [Header("Prefab Settings")]
        [SerializeField] private GameObject _gemPrefab; // Gem的预制体（包含GemMonoBehaviour组件）
        
        [Header("Board Settings")]
        [SerializeField] private int _boardRows = 8;
        [SerializeField] private int _boardCols = 8;
        [SerializeField] private float _cellSize = 1f;
        [SerializeField] private Vector3 _boardStartPosition = Vector3.zero;
        
        [Header("Visual Settings")]
        [SerializeField] private Color _matchHighlightColor = Color.white;
        
        private IMatch3Engine _engine;
        private GemMonoBehaviour[,] _gemViews;
        private GemMonoBehaviour _selectedGem;
        
        void Start()
        {
            InitializeGame();
        }
        
        /// <summary>
        /// 初始化游戏
        /// </summary>
        void InitializeGame()
        {
            // 初始化引擎
            _engine = new Match3GameEngine();
            _engine.Initialize(_boardRows, _boardCols);
            
            // 订阅事件
            _engine.OnMatchFound += OnMatchFound;
            _engine.OnEliminationComplete += OnEliminationComplete;
            _engine.OnFallComplete += OnFallComplete;
            _engine.OnScoreChanged += OnScoreChanged;
            _engine.OnGameOver += OnGameOver;
            
            // 创建UI视图
            CreateBoardUI();
            
            Debug.Log("游戏已初始化！");
        }
        
        /// <summary>
        /// 创建游戏板UI
        /// </summary>
        void CreateBoardUI()
        {
            _gemViews = new GemMonoBehaviour[_boardRows, _boardCols];
            
            // 遍历所有位置创建Gem视图
            foreach (var position in _engine.Board.GetAllPositions())
            {
                var gem = _engine.Board.GetGem(position);
                if (gem != null)
                {
                    CreateGemView(gem, position);
                }
            }
        }
        
        /// <summary>
        /// 创建单个Gem的视图
        /// </summary>
        void CreateGemView(IGem gem, BoardPosition position)
        {
            GameObject gemObject;
            
            // 使用预制体或创建新对象
            if (_gemPrefab != null)
            {
                gemObject = Instantiate(_gemPrefab, transform);
            }
            else
            {
                // 如果没有预制体，创建一个简单的GameObject
                gemObject = new GameObject($"Gem_{position}");
                gemObject.transform.SetParent(transform);
                
                // 添加SpriteRenderer
                var spriteRenderer = gemObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = CreateSpriteForGem(gem.Type);
                
                // 添加GemMonoBehaviour组件
                var gemView = gemObject.AddComponent<GemMonoBehaviour>();
                _gemViews[position.Row, position.Col] = gemView;
            }
            
            // 获取或添加GemMonoBehaviour组件
            var gemMono = gemObject.GetComponent<GemMonoBehaviour>();
            if (gemMono == null)
            {
                gemMono = gemObject.AddComponent<GemMonoBehaviour>();
            }
            
            // 初始化Gem数据
            gemMono.Initialize(gem);
            
            // 订阅点击事件
            gemMono.OnGemClicked.AddListener(OnGemClicked);
            
            // 设置位置
            Vector3 worldPosition = BoardPositionToWorldPosition(position);
            gemMono.SetPosition(worldPosition);
            
            // 添加到数组
            _gemViews[position.Row, position.Col] = gemMono;
        }
        
        /// <summary>
        /// 将游戏板位置转换为世界坐标
        /// </summary>
        Vector3 BoardPositionToWorldPosition(BoardPosition position)
        {
            float x = _boardStartPosition.x + position.Col * _cellSize;
            float y = _boardStartPosition.y - position.Row * _cellSize; // Y轴向下（或根据你的坐标系调整）
            return new Vector3(x, y, _boardStartPosition.z);
        }
        
        /// <summary>
        /// 创建宝石的Sprite（示例：如果没有预制体，创建一个彩色方块）
        /// </summary>
        Sprite CreateSpriteForGem(GemType gemType)
        {
            // 这里可以创建简单的彩色方块Sprite
            // 实际项目中应该使用准备好的Sprite资源
            Texture2D texture = new Texture2D(32, 32);
            Color color = GetGemColor(gemType);
            
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            
            return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        }
        
        /// <summary>
        /// 根据宝石类型获取颜色
        /// </summary>
        Color GetGemColor(GemType gemType)
        {
            return gemType switch
            {
                GemType.Red => Color.red,
                GemType.Blue => Color.blue,
                GemType.Green => Color.green,
                GemType.Yellow => Color.yellow,
                GemType.Purple => new Color(0.5f, 0f, 0.5f),
                GemType.Orange => new Color(1f, 0.5f, 0f),
                GemType.Special => Color.magenta,
                _ => Color.white
            };
        }
        
        /// <summary>
        /// 处理Gem点击事件
        /// </summary>
        void OnGemClicked(GemMonoBehaviour clickedGem)
        {
            if (_selectedGem == null)
            {
                // 选择第一个Gem
                _selectedGem = clickedGem;
                _selectedGem.IsSelected = true;
                Debug.Log($"选中Gem: {clickedGem.Position}");
            }
            else
            {
                // 尝试交换
                if (_selectedGem == clickedGem)
                {
                    // 取消选择
                    _selectedGem.IsSelected = false;
                    _selectedGem = null;
                }
                else
                {
                    // 尝试交换两个Gem
                    TrySwapGems(_selectedGem, clickedGem);
                    
                    // 取消选择
                    _selectedGem.IsSelected = false;
                    _selectedGem = null;
                }
            }
        }
        
        /// <summary>
        /// 尝试交换两个Gem
        /// </summary>
        void TrySwapGems(GemMonoBehaviour gem1, GemMonoBehaviour gem2)
        {
            var swapResult = _engine.TrySwap(gem1.Position, gem2.Position);
            
            if (swapResult.Success)
            {
                Debug.Log($"成功交换 {gem1.Position} 和 {gem2.Position}");
                
                // 执行交换动画
                Vector3 pos1 = gem1.transform.position;
                Vector3 pos2 = gem2.transform.position;
                
                gem1.AnimateSwap(pos2, () => UpdateGemPosition(gem1));
                gem2.AnimateSwap(pos1, () => UpdateGemPosition(gem2));
            }
            else
            {
                Debug.Log($"交换失败: {swapResult.Reason}");
            }
        }
        
        /// <summary>
        /// 更新Gem位置（交换后）
        /// </summary>
        void UpdateGemPosition(GemMonoBehaviour gemView)
        {
            Vector3 targetPos = BoardPositionToWorldPosition(gemView.Position);
            gemView.SetTargetPosition(targetPos);
        }
        
        /// <summary>
        /// 更新整个游戏板的视图
        /// </summary>
        void UpdateBoardView()
        {
            foreach (var position in _engine.Board.GetAllPositions())
            {
                var gem = _engine.Board.GetGem(position);
                var gemView = _gemViews[position.Row, position.Col];
                
                if (gem == null && gemView != null)
                {
                    // Gem被消除，播放消除动画
                    gemView.AnimateEliminate(() => DestroyGemView(gemView, position));
                }
                else if (gem != null && gemView == null)
                {
                    // 新Gem生成
                    CreateGemView(gem, position);
                    var newGemView = _gemViews[position.Row, position.Col];
                    if (newGemView != null)
                    {
                        newGemView.transform.localScale = Vector3.zero;
                        newGemView.AnimateAppear();
                    }
                }
                else if (gem != null && gemView != null)
                {
                    // 更新位置（下落）
                    UpdateGemPosition(gemView);
                }
            }
        }
        
        /// <summary>
        /// 销毁Gem视图
        /// </summary>
        void DestroyGemView(GemMonoBehaviour gemView, BoardPosition position)
        {
            if (gemView != null)
            {
                _gemViews[position.Row, position.Col] = null;
                Destroy(gemView.gameObject);
            }
        }
        
        void Update()
        {
            // 更新游戏逻辑
            _engine?.Update(Time.deltaTime);
        }
        
        // 事件处理
        private void OnMatchFound(MatchResult match)
        {
            Debug.Log($"检测到匹配！类型: {match.GemType}, 长度: {match.MatchLength}");
            
            // 高亮匹配的Gem
            foreach (var pos in match.MatchedPositions)
            {
                var gemView = _gemViews[pos.Row, pos.Col];
                if (gemView != null)
                {
                    gemView.SetHighlight(true, _matchHighlightColor);
                }
            }
        }
        
        private void OnEliminationComplete()
        {
            Debug.Log("消除完成");
            UpdateBoardView();
        }
        
        private void OnFallComplete()
        {
            Debug.Log("下落完成");
            UpdateBoardView();
        }
        
        private void OnScoreChanged(int newScore)
        {
            Debug.Log($"分数更新: {newScore} (连击: {_engine.ComboCount})");
        }
        
        private void OnGameOver()
        {
            Debug.Log($"游戏结束！最终分数: {_engine.Score}");
        }
        
        void OnDestroy()
        {
            // 取消订阅事件
            if (_engine != null)
            {
                _engine.OnMatchFound -= OnMatchFound;
                _engine.OnEliminationComplete -= OnEliminationComplete;
                _engine.OnFallComplete -= OnFallComplete;
                _engine.OnScoreChanged -= OnScoreChanged;
                _engine.OnGameOver -= OnGameOver;
            }
        }
    }
}
