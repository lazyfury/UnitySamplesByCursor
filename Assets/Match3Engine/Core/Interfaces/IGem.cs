namespace Match3Engine.Core.Interfaces
{
    using Match3Engine.Core.Types;
    
    /// <summary>
    /// 宝石接口
    /// </summary>
    public interface IGem
    {
        /// <summary>
        /// 宝石类型
        /// </summary>
        GemType Type { get; set; }
        
        /// <summary>
        /// 宝石位置
        /// </summary>
        BoardPosition Position { get; set; }
        
        /// <summary>
        /// 是否为特殊宝石
        /// </summary>
        bool IsSpecial { get; }
        
        /// <summary>
        /// 宝石唯一ID
        /// </summary>
        int Id { get; }
    }
}
