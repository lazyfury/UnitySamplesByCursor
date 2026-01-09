using System;
using System.Collections.Generic;
using PokerEngine.Core.Types;

namespace PokerEngine.Core.Interfaces
{
    /// <summary>
    /// 德州扑克引擎接口
    /// </summary>
    public interface IPokerEngine
    {
        GameState State { get; }
        GamePhase Phase { get; }
        int SmallBlind { get; set; }
        int BigBlind { get; set; }
        int CurrentPot { get; }
        int CurrentBet { get; }
        int DealerPosition { get; }
        int CurrentPlayerPosition { get; }
        List<Card> CommunityCards { get; }
        List<IPokerPlayer> Players { get; }
        List<IPokerPlayer> ActivePlayers { get; }

        // 事件
        event Action<GamePhase> OnPhaseChanged;
        event Action<int> OnPotChanged;
        event Action<IPokerPlayer, PlayerAction> OnPlayerAction;
        event Action<List<IPokerPlayer>> OnHandFinished;
        event Action<IPokerPlayer> OnPlayerEliminated;

        // 游戏控制
        void Initialize(List<IPokerPlayer> players, int smallBlind, int bigBlind);
        void StartNewHand();
        void ProcessPlayerAction(int playerId, ActionType actionType, int amount = 0);
        void NextPhase();
        void EndHand();
        void RemovePlayer(int playerId);
        HandResult EvaluateHand(List<Card> holeCards, List<Card> communityCards);
        List<IPokerPlayer> GetWinners();
    }
}
