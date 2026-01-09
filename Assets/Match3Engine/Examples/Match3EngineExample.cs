using UnityEngine;
using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Types;
using Match3Engine.Core.Implementations;

namespace Match3Engine.Examples
{
    /// <summary>
    /// 三消游戏引擎使用示例
    /// </summary>
    public class Match3EngineExample : MonoBehaviour
    {
        private IMatch3Engine _engine;
        
        private BoardPosition? _selectedPosition;
        
        void Start()
        {
            // 初始化游戏引擎（8x8的游戏板）
            _engine = new Match3GameEngine();
            _engine.Initialize(8, 8);
            
            // 设置移动次数限制（可选）
            _engine.RemainingMoves = 30;
            
            // 订阅事件
            _engine.OnMatchFound += OnMatchFound;
            _engine.OnEliminationComplete += OnEliminationComplete;
            _engine.OnFallComplete += OnFallComplete;
            _engine.OnScoreChanged += OnScoreChanged;
            _engine.OnGameOver += OnGameOver;
            
            Debug.Log("三消游戏已初始化！");
            Debug.Log($"当前分数: {_engine.Score}");
        }
        
        void Update()
        {
            // 更新游戏逻辑
            _engine.Update(Time.deltaTime);
            
            // 示例：处理鼠标点击输入
            if (Input.GetMouseButtonDown(0))
            {
                HandleInput();
            }
            
            // 示例：键盘输入（重置游戏）
            if (Input.GetKeyDown(KeyCode.R))
            {
                _engine.Reset();
                Debug.Log("游戏已重置");
            }
            
            // 示例：暂停/恢复
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (_engine.State == GameState.Paused)
                {
                    _engine.Resume();
                    Debug.Log("游戏已恢复");
                }
                else
                {
                    _engine.Pause();
                    Debug.Log("游戏已暂停");
                }
            }
        }
        
        private void HandleInput()
        {
            // 这里应该根据实际的UI系统将屏幕坐标转换为游戏板位置
            // 示例代码：
            /*
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            
            // 将世界坐标转换为游戏板坐标（需要根据实际布局调整）
            int col = Mathf.FloorToInt(worldPos.x);
            int row = Mathf.FloorToInt(worldPos.y);
            
            BoardPosition position = new BoardPosition(row, col);
            
            if (!_engine.Board.IsValidPosition(position))
                return;
            
            if (_selectedPosition == null)
            {
                // 选择第一个位置
                _selectedPosition = position;
                Debug.Log($"选择位置: {position}");
            }
            else
            {
                // 尝试交换
                var swapResult = _engine.TrySwap(_selectedPosition.Value, position);
                
                if (swapResult.Success)
                {
                    Debug.Log($"成功交换 {_selectedPosition.Value} 和 {position}");
                }
                else
                {
                    Debug.Log($"交换失败: {swapResult.Reason}");
                }
                
                _selectedPosition = null;
            }
            */
        }
        
        private void OnMatchFound(MatchResult match)
        {
            Debug.Log($"检测到匹配！类型: {match.GemType}, 长度: {match.MatchLength}, " +
                     $"方向: {(match.IsHorizontal ? "横向" : "纵向")}, " +
                     $"位置数: {match.MatchedPositions.Count}");
        }
        
        private void OnEliminationComplete()
        {
            Debug.Log("消除完成");
        }
        
        private void OnFallComplete()
        {
            Debug.Log("下落完成");
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
        
        /// <summary>
        /// 在Unity编辑器中绘制游戏板（仅用于调试）
        /// </summary>
        void OnDrawGizmos()
        {
            if (_engine?.Board == null)
                return;
            
            // 绘制游戏板网格（需要根据实际位置调整）
            Gizmos.color = Color.white;
            for (int row = 0; row < _engine.Board.Rows; row++)
            {
                for (int col = 0; col < _engine.Board.Cols; col++)
                {
                    var pos = new BoardPosition(row, col);
                    var gem = _engine.Board.GetGem(pos);
                    
                    if (gem != null)
                    {
                        // 根据宝石类型设置颜色
                        Gizmos.color = GetGemColor(gem.Type);
                        
                        // 绘制宝石位置（需要根据实际坐标转换）
                        Vector3 worldPos = new Vector3(col, row, 0);
                        Gizmos.DrawCube(worldPos, Vector3.one * 0.8f);
                    }
                }
            }
        }
        
        private Color GetGemColor(GemType gemType)
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
    }
}
