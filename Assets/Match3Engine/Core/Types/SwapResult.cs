namespace Match3Engine.Core.Types
{
    /// <summary>
    /// 交换结果
    /// </summary>
    public class SwapResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// 是否产生了匹配
        /// </summary>
        public bool HasMatch { get; set; }
        
        /// <summary>
        /// 失败原因（如果Success为false）
        /// </summary>
        public string Reason { get; set; }
        
        public SwapResult()
        {
            Success = false;
            HasMatch = false;
            Reason = string.Empty;
        }
        
        public SwapResult(bool success, bool hasMatch, string reason = "")
        {
            Success = success;
            HasMatch = hasMatch;
            Reason = reason;
        }
    }
}
