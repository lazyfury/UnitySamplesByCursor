using System;
using System.Collections.Generic;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Interfaces
{
    /// <summary>
    /// 三国杀风格卡牌战斗引擎接口
    /// </summary>
    public interface ISGSKEngine
    {
        /// <summary>
        /// 游戏状态
        /// </summary>
        GameState State { get; }
        
        /// <summary>
        /// 当前游戏阶段
        /// </summary>
        GamePhase Phase { get; }
        
        /// <summary>
        /// 当前回合玩家ID
        /// </summary>
        int CurrentPlayerId { get; }
        
        /// <summary>
        /// 所有玩家列表
        /// </summary>
        List<IPlayer> Players { get; }
        
        /// <summary>
        /// 存活玩家列表
        /// </summary>
        List<IPlayer> AlivePlayers { get; }
        
        /// <summary>
        /// 牌堆
        /// </summary>
        IDeck Deck { get; }
        
        /// <summary>
        /// 弃牌堆
        /// </summary>
        List<Card> DiscardPile { get; }

        // 事件
        event Action<GamePhase> OnPhaseChanged;
        event Action<int> OnTurnStarted;
        event Action<int> OnTurnEnded;
        event Action<IPlayer, Card> OnCardPlayed;
        event Action<IPlayer, Card> OnCardUsed;
        event Action<DamageResult> OnDamageDealt;
        event Action<int, int> OnPlayerDied;
        event Action<IPlayer> OnPlayerStateChanged;

        // 游戏控制
        void Initialize(List<IPlayer> players);
        void StartGame();
        void EndGame();
        
        // 回合控制
        void StartTurn(int playerId);
        void EndTurn();
        void NextPhase();
        
        // 卡牌操作
        bool PlayCard(int playerId, int cardId, int? targetId = null);
        bool UseCard(int playerId, int cardId, int? targetId = null);
        void DiscardCard(int playerId, int cardId);
        void DrawCard(int playerId, int count = 1);
        
        // 战斗
        bool Attack(int sourceId, int targetId, int? cardId = null);
        bool Dodge(int playerId, int cardId);
        DamageResult DealDamage(int sourceId, int targetId, int amount, DamageType type = DamageType.Normal);
        bool RecoverHp(int playerId, int amount);
        
        // 判定
        Card Judge(int playerId);
        void ProcessDelayedTrick(int playerId);
        
        // 距离计算
        int GetDistance(int fromId, int toId);
        bool IsInRange(int fromId, int toId);
        
        // 查询
        IPlayer GetPlayer(int playerId);
        List<Card> GetPlayerHand(int playerId);
        List<Card> GetPlayerEquipments(int playerId);
    }
}
