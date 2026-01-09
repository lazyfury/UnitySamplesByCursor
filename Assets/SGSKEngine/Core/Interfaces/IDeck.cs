using System.Collections.Generic;
using SGSKEngine.Core.Types;

namespace SGSKEngine.Core.Interfaces
{
    /// <summary>
    /// 牌堆接口
    /// </summary>
    public interface IDeck
    {
        /// <summary>
        /// 剩余牌数
        /// </summary>
        int Count { get; }
        
        /// <summary>
        /// 是否为空
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 洗牌
        /// </summary>
        void Shuffle();
        
        /// <summary>
        /// 抽牌
        /// </summary>
        Card Draw();
        
        /// <summary>
        /// 抽多张牌
        /// </summary>
        List<Card> Draw(int count);
        
        /// <summary>
        /// 添加牌到底部
        /// </summary>
        void AddCard(Card card);
        
        /// <summary>
        /// 添加多张牌
        /// </summary>
        void AddCards(List<Card> cards);
        
        /// <summary>
        /// 查看顶部牌（不抽取）
        /// </summary>
        Card Peek();
        
        /// <summary>
        /// 初始化标准牌堆
        /// </summary>
        void InitializeStandardDeck();
    }
}
