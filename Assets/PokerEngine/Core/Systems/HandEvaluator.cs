using System;
using System.Collections.Generic;
using System.Linq;
using PokerEngine.Core.Types;

namespace PokerEngine.Core.Systems
{
    /// <summary>
    /// 手牌评估系统
    /// </summary>
    public static class HandEvaluator
    {
        /// <summary>
        /// 评估手牌（2张底牌 + 5张公共牌，选出最好的5张）
        /// </summary>
        public static HandResult EvaluateHand(List<Card> holeCards, List<Card> communityCards)
        {
            if (holeCards == null || holeCards.Count != 2)
                throw new ArgumentException("必须提供2张底牌");
            if (communityCards == null || communityCards.Count < 3 || communityCards.Count > 5)
                throw new ArgumentException("公共牌必须是3-5张");

            var allCards = new List<Card>(holeCards);
            allCards.AddRange(communityCards);

            // 生成所有可能的5张牌组合
            var bestHand = GetBestFiveCardHand(allCards);

            return bestHand;
        }

        /// <summary>
        /// 从7张牌中选出最好的5张牌组合
        /// </summary>
        private static HandResult GetBestFiveCardHand(List<Card> cards)
        {
            if (cards.Count < 5)
                throw new ArgumentException("至少需要5张牌");

            HandResult bestHand = null;

            // 生成所有5张牌的组合
            var combinations = GetCombinations(cards, 5);
            foreach (var combo in combinations)
            {
                var result = EvaluateFiveCardHand(combo);
                if (bestHand == null || CompareHands(result, bestHand) > 0)
                {
                    bestHand = result;
                }
            }

            return bestHand;
        }

        /// <summary>
        /// 评估5张牌的手牌
        /// </summary>
        private static HandResult EvaluateFiveCardHand(List<Card> cards)
        {
            if (cards.Count != 5)
                throw new ArgumentException("必须正好5张牌");

            var sortedCards = cards.OrderBy(c => c.Rank).ToList();
            var ranks = sortedCards.Select(c => (int)c.Rank).ToList();
            var suits = sortedCards.Select(c => c.Suit).ToList();

            // 检查是否是同花
            bool isFlush = suits.Distinct().Count() == 1;

            // 检查是否是顺子（返回是否为顺子以及高牌值）
            var straightResult = IsStraightWithHighCard(ranks);
            bool isStraight = straightResult.isStraight;
            int straightHighCard = straightResult.highCard;

            // 检查是否是皇家同花顺
            if (isFlush && isStraight && straightHighCard == 14)
            {
                return new HandResult(HandRank.RoyalFlush, sortedCards, 10000000);
            }

            // 检查是否是同花顺
            if (isFlush && isStraight)
            {
                return new HandResult(HandRank.StraightFlush, sortedCards, 9000000 + straightHighCard);
            }

            // 统计相同点数的牌
            var rankGroups = ranks.GroupBy(r => r).OrderByDescending(g => g.Count()).ThenByDescending(g => g.Key).ToList();

            // 检查是否是四条
            if (rankGroups[0].Count() == 4)
            {
                int fourRank = rankGroups[0].Key;
                int kicker = rankGroups[1].Key;
                return new HandResult(HandRank.FourOfAKind, sortedCards, 8000000 + fourRank * 100 + kicker);
            }

            // 检查是否是葫芦
            if (rankGroups[0].Count() == 3 && rankGroups[1].Count() == 2)
            {
                int threeRank = rankGroups[0].Key;
                int pairRank = rankGroups[1].Key;
                return new HandResult(HandRank.FullHouse, sortedCards, 7000000 + threeRank * 100 + pairRank);
            }

            // 检查是否是同花
            if (isFlush)
            {
                int score = 6000000;
                for (int i = 0; i < 5; i++)
                {
                    score += ranks[i] * (int)Math.Pow(14, 4 - i);
                }
                return new HandResult(HandRank.Flush, sortedCards, score);
            }

            // 检查是否是顺子
            if (isStraight)
            {
                return new HandResult(HandRank.Straight, sortedCards, 5000000 + straightHighCard);
            }

            // 检查是否是三条
            if (rankGroups[0].Count() == 3)
            {
                int threeRank = rankGroups[0].Key;
                int score = 4000000 + threeRank * 10000;
                int kickerIndex = 1;
                for (int i = 0; i < 2 && kickerIndex < rankGroups.Count; i++)
                {
                    score += rankGroups[kickerIndex].Key * (int)Math.Pow(14, 1 - i);
                    kickerIndex++;
                }
                return new HandResult(HandRank.ThreeOfAKind, sortedCards, score);
            }

            // 检查是否是两对
            if (rankGroups[0].Count() == 2 && rankGroups[1].Count() == 2)
            {
                int pair1Rank = rankGroups[0].Key;
                int pair2Rank = rankGroups[1].Key;
                int kicker = rankGroups.Count > 2 ? rankGroups[2].Key : 0;
                return new HandResult(HandRank.TwoPair, sortedCards, 3000000 + Math.Max(pair1Rank, pair2Rank) * 10000 + Math.Min(pair1Rank, pair2Rank) * 100 + kicker);
            }

            // 检查是否是一对
            if (rankGroups[0].Count() == 2)
            {
                int pairRank = rankGroups[0].Key;
                int score = 2000000 + pairRank * 10000;
                int kickerIndex = 1;
                for (int i = 0; i < 3 && kickerIndex < rankGroups.Count; i++)
                {
                    score += rankGroups[kickerIndex].Key * (int)Math.Pow(14, 2 - i);
                    kickerIndex++;
                }
                return new HandResult(HandRank.Pair, sortedCards, score);
            }

            // 高牌
            int highCardScore = 1000000;
            for (int i = 0; i < 5; i++)
            {
                highCardScore += ranks[i] * (int)Math.Pow(14, 4 - i);
            }
            return new HandResult(HandRank.HighCard, sortedCards, highCardScore);
        }

        /// <summary>
        /// 检查是否是顺子，并返回高牌值
        /// </summary>
        private static (bool isStraight, int highCard) IsStraightWithHighCard(List<int> ranks)
        {
            var sortedRanks = ranks.OrderBy(r => r).ToList();
            
            // 处理A-2-3-4-5的特殊情况（A作为1，高牌是5）
            if (sortedRanks.SequenceEqual(new List<int> { 1, 2, 3, 4, 5 }))
                return (true, 5);

            // 处理10-J-Q-K-A的情况（A作为14，高牌是14）
            if (sortedRanks.SequenceEqual(new List<int> { 1, 10, 11, 12, 13 }))
                return (true, 14);

            // 普通顺子
            bool isStraight = true;
            for (int i = 0; i < sortedRanks.Count - 1; i++)
            {
                if (sortedRanks[i + 1] - sortedRanks[i] != 1)
                {
                    isStraight = false;
                    break;
                }
            }
            
            if (isStraight)
            {
                return (true, sortedRanks.Max());
            }
            
            return (false, 0);
        }

        /// <summary>
        /// 比较两手牌，返回正数表示hand1更好，负数表示hand2更好，0表示相等
        /// </summary>
        public static int CompareHands(HandResult hand1, HandResult hand2)
        {
            if (hand1.Rank != hand2.Rank)
            {
                return hand1.Rank.CompareTo(hand2.Rank);
            }
            return hand1.Score.CompareTo(hand2.Score);
        }

        /// <summary>
        /// 生成组合
        /// </summary>
        private static List<List<Card>> GetCombinations(List<Card> cards, int k)
        {
            var result = new List<List<Card>>();
            var current = new List<Card>();
            GenerateCombinations(cards, 0, k, current, result);
            return result;
        }

        private static void GenerateCombinations(List<Card> cards, int start, int k, List<Card> current, List<List<Card>> result)
        {
            if (current.Count == k)
            {
                result.Add(new List<Card>(current));
                return;
            }

            for (int i = start; i < cards.Count; i++)
            {
                current.Add(cards[i]);
                GenerateCombinations(cards, i + 1, k, current, result);
                current.RemoveAt(current.Count - 1);
            }
        }
    }
}
