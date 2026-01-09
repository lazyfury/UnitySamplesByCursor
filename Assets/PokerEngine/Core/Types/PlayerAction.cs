namespace PokerEngine.Core.Types
{
    /// <summary>
    /// 玩家行动
    /// </summary>
    public class PlayerAction
    {
        public int PlayerId { get; }
        public ActionType ActionType { get; }
        public int Amount { get; } // 下注/加注金额

        public PlayerAction(int playerId, ActionType actionType, int amount = 0)
        {
            PlayerId = playerId;
            ActionType = actionType;
            Amount = amount;
        }
    }
}
