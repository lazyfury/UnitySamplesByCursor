using System.Collections.Generic;
using System.Linq;
using PokerEngine.Core.Interfaces;
using PokerEngine.Core.Types;

namespace PokerEngine.Core.Systems
{
    /// <summary>
    /// 底池管理系统
    /// </summary>
    public class PotManager
    {
        private int _mainPot;
        private List<SidePot> _sidePots;

        public int MainPot => _mainPot;
        public List<SidePot> SidePots => new List<SidePot>(_sidePots);
        public int TotalPot => _mainPot + _sidePots.Sum(sp => sp.Amount);

        public PotManager()
        {
            _mainPot = 0;
            _sidePots = new List<SidePot>();
        }

        public void Reset()
        {
            _mainPot = 0;
            _sidePots.Clear();
        }

        public void CollectBets(List<IPokerPlayer> players)
        {
            // 计算每个玩家的总下注
            var playerBets = players.ToDictionary(p => p.Id, p => p.CurrentBet);

            // 找出所有不同的下注金额（用于创建边池）
            var betLevels = playerBets.Values.Where(b => b > 0).Distinct().OrderBy(b => b).ToList();

            if (betLevels.Count == 0)
                return;

            // 创建主池和边池
            int previousLevel = 0;
            foreach (var level in betLevels)
            {
                int potAmount = 0;
                var eligiblePlayers = new List<int>();

                foreach (var player in players)
                {
                    if (player.CurrentBet >= level)
                    {
                        potAmount += level - previousLevel;
                        if (player.CurrentBet >= level && !player.HasFolded)
                        {
                            eligiblePlayers.Add(player.Id);
                        }
                    }
                }

                if (previousLevel == 0)
                {
                    _mainPot = potAmount;
                }
                else
                {
                    _sidePots.Add(new SidePot
                    {
                        Amount = potAmount,
                        EligiblePlayerIds = eligiblePlayers
                    });
                }

                previousLevel = level;
            }

            // 重置玩家下注
            foreach (var player in players)
            {
                player.CurrentBet = 0;
            }
        }

        public void DistributePot(List<IPokerPlayer> winners, Dictionary<int, HandResult> handResults)
        {
            // 分配主池
            DistributePotToWinners(_mainPot, winners, handResults);

            // 分配边池
            foreach (var sidePot in _sidePots)
            {
                var eligibleWinners = winners.Where(w => sidePot.EligiblePlayerIds.Contains(w.Id)).ToList();
                if (eligibleWinners.Count > 0)
                {
                    DistributePotToWinners(sidePot.Amount, eligibleWinners, handResults);
                }
            }
        }

        private void DistributePotToWinners(int potAmount, List<IPokerPlayer> winners, Dictionary<int, HandResult> handResults)
        {
            if (winners.Count == 0)
                return;

            int chipsPerWinner = potAmount / winners.Count;
            int remainder = potAmount % winners.Count;

            foreach (var winner in winners)
            {
                winner.Chips += chipsPerWinner;
                if (remainder > 0)
                {
                    winner.Chips += 1;
                    remainder--;
                }
            }
        }
    }

    /// <summary>
    /// 边池
    /// </summary>
    public class SidePot
    {
        public int Amount { get; set; }
        public List<int> EligiblePlayerIds { get; set; }

        public SidePot()
        {
            EligiblePlayerIds = new List<int>();
        }
    }
}
