using System;
using System.Collections.Generic;
using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Systems;
using Match3Engine.Core.Types;

namespace Match3Engine.Core.Implementations
{
    /// <summary>
    /// 三消游戏引擎实现
    /// </summary>
    public class Match3Engine : IMatch3Engine
    {
        private MatchDetector _matchDetector;
        private MatchEliminator _matchEliminator;
        private FallSystem _fallSystem;
        private SwapSystem _swapSystem;
        
        private float _stateTimer;
        private const float STATE_CHANGE_DELAY = 0.1f; // 状态转换延迟（秒）
        
        private List<MatchResult> _pendingMatches;
        private int _comboMultiplier;
        private bool _isProcessingChain;
        
        public GameState State { get; private set; }
        public IGameBoard Board { get; private set; }
        public int Score { get; private set; }
        public int ComboCount { get; private set; }
        public int RemainingMoves { get; set; }
        
        public event Action<MatchResult> OnMatchFound;
        public event Action OnEliminationComplete;
        public event Action OnFallComplete;
        public event Action<int> OnScoreChanged;
        public event Action OnGameOver;
        
        public Match3Engine()
        {
            _matchDetector = new MatchDetector();
            _matchEliminator = new MatchEliminator();
            _fallSystem = new FallSystem();
            _swapSystem = new SwapSystem();
            
            State = GameState.NotInitialized;
            _pendingMatches = new List<MatchResult>();
            _comboMultiplier = 1;
            _isProcessingChain = false;
        }
        
        public void Initialize(int rows, int cols)
        {
            if (rows < 3 || cols < 3)
            {
                throw new ArgumentException("游戏板大小至少为3x3");
            }
            
            Board = new GameBoard(rows, cols);
            Board.Initialize();
            
            State = GameState.Ready;
            Score = 0;
            ComboCount = 0;
            _comboMultiplier = 1;
            _isProcessingChain = false;
            _stateTimer = 0;
            
            // 确保初始状态没有匹配
            EnsureNoInitialMatches();
        }
        
        private void EnsureNoInitialMatches()
        {
            var matches = _matchDetector.DetectAllMatches(Board);
            int attempts = 0;
            const int maxAttempts = 100;
            
            while (matches.Count > 0 && attempts < maxAttempts)
            {
                Board.Initialize();
                matches = _matchDetector.DetectAllMatches(Board);
                attempts++;
            }
        }
        
        public SwapResult TrySwap(BoardPosition pos1, BoardPosition pos2)
        {
            if (State != GameState.Ready && State != GameState.WaitingForInput)
            {
                return new SwapResult(false, false, "游戏状态不允许交换");
            }
            
            var swapResult = _swapSystem.ExecuteSwap(Board, pos1, pos2);
            
            if (swapResult.Success && swapResult.HasMatch)
            {
                State = GameState.Checking;
                _stateTimer = 0;
                _comboMultiplier = 1;
                _isProcessingChain = true;
                
                // 开始处理匹配链
                ProcessMatchChain();
            }
            else if (swapResult.Success)
            {
                // 这不应该发生，但如果发生了，恢复状态
                State = GameState.WaitingForInput;
            }
            
            return swapResult;
        }
        
        private void ProcessMatchChain()
        {
            while (true)
            {
                // 检测所有匹配
                var matches = _matchDetector.DetectAllMatches(Board);
                matches = _matchDetector.MergeMatches(matches);
                
                if (matches.Count == 0)
                {
                    // 没有更多匹配，结束链
                    _isProcessingChain = false;
                    _comboMultiplier = 1;
                    ComboCount = 0;
                    
                    if (RemainingMoves > 0)
                    {
                        RemainingMoves--;
                        if (RemainingMoves <= 0)
                        {
                            State = GameState.GameOver;
                            OnGameOver?.Invoke();
                            return;
                        }
                    }
                    
                    State = GameState.Ready;
                    break;
                }
                
                // 处理匹配
                foreach (var match in matches)
                {
                    OnMatchFound?.Invoke(match);
                }
                
                // 计算分数
                int matchScore = _matchEliminator.CalculateScore(matches, _comboMultiplier);
                Score += matchScore;
                OnScoreChanged?.Invoke(Score);
                
                // 消除匹配
                _matchEliminator.EliminateMatches(Board, matches);
                OnEliminationComplete?.Invoke();
                
                // 增加连击
                if (_isProcessingChain)
                {
                    ComboCount++;
                    _comboMultiplier = Math.Min(_comboMultiplier + 1, 5); // 最大5倍
                }
                
                // 执行下落
                bool hasFallen = _fallSystem.ExecuteFall(Board);
                if (hasFallen)
                {
                    OnFallComplete?.Invoke();
                }
                
                // 填充空位
                _fallSystem.FillEmptySpaces(Board);
                
                // 继续循环检查新的匹配（连击）
            }
        }
        
        public void Update(float deltaTime)
        {
            if (State == GameState.Paused || State == GameState.NotInitialized || State == GameState.GameOver)
                return;
            
            _stateTimer += deltaTime;
            
            // 状态机逻辑
            switch (State)
            {
                case GameState.Ready:
                    // 等待玩家输入
                    break;
                    
                case GameState.Checking:
                    if (_stateTimer >= STATE_CHANGE_DELAY)
                    {
                        _stateTimer = 0;
                        // 检查逻辑在ProcessMatchChain中处理
                    }
                    break;
                    
                case GameState.Eliminating:
                    if (_stateTimer >= STATE_CHANGE_DELAY)
                    {
                        _stateTimer = 0;
                        State = GameState.Falling;
                    }
                    break;
                    
                case GameState.Falling:
                    if (_stateTimer >= STATE_CHANGE_DELAY)
                    {
                        _stateTimer = 0;
                        State = GameState.Filling;
                    }
                    break;
                    
                case GameState.Filling:
                    if (_stateTimer >= STATE_CHANGE_DELAY)
                    {
                        _stateTimer = 0;
                        State = GameState.Checking;
                    }
                    break;
            }
        }
        
        public void Pause()
        {
            if (State != GameState.Paused && State != GameState.NotInitialized && State != GameState.GameOver)
            {
                State = GameState.Paused;
            }
        }
        
        public void Resume()
        {
            if (State == GameState.Paused)
            {
                State = GameState.WaitingForInput;
            }
        }
        
        public void Reset()
        {
            if (Board != null)
            {
                Board.Initialize();
                EnsureNoInitialMatches();
            }
            
            State = GameState.Ready;
            Score = 0;
            ComboCount = 0;
            _comboMultiplier = 1;
            _isProcessingChain = false;
            _stateTimer = 0;
            
            OnScoreChanged?.Invoke(Score);
        }
    }
}
