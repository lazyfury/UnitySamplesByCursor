namespace PokerEngine.Core.Types
{
    /// <summary>
    /// 玩家行动类型
    /// </summary>
    public enum ActionType
    {
        Fold,        // 弃牌
        Check,       // 过牌
        Call,        // 跟注
        Bet,         // 下注
        Raise,       // 加注
        AllIn        // 全押
    }
}
