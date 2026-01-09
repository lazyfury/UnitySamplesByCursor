using System;
using System.Collections.Generic;
using System.Linq;
using WarStrategyEngine.Core.Interfaces;

namespace WarStrategyEngine.Core.Systems
{
    /// <summary>
    /// A*路径查找算法
    /// </summary>
    public class Pathfinding
    {
        private class Node
        {
            public Position Position { get; set; }
            public float G { get; set; } // 从起点到当前节点的实际代价
            public float H { get; set; } // 从当前节点到终点的启发式估计代价
            public float F => G + H;     // 总代价
            public Node Parent { get; set; }
            
            public Node(Position position)
            {
                Position = position;
                G = 0;
                H = 0;
                Parent = null;
            }
        }
        
        /// <summary>
        /// 查找路径
        /// </summary>
        public static List<Position> FindPath(IMap map, Position start, Position goal, IUnit unit)
        {
            if (!map.IsValidPosition(start) || !map.IsValidPosition(goal))
                return new List<Position>();
            
            if (start == goal)
                return new List<Position> { start };
            
            var openSet = new List<Node>();
            var closedSet = new HashSet<Position>();
            var allNodes = new Dictionary<Position, Node>();
            
            var startNode = new Node(start);
            startNode.G = 0;
            startNode.H = Heuristic(start, goal);
            openSet.Add(startNode);
            allNodes[start] = startNode;
            
            while (openSet.Count > 0)
            {
                // 获取F值最小的节点
                var currentNode = openSet.OrderBy(n => n.F).First();
                openSet.Remove(currentNode);
                closedSet.Add(currentNode.Position);
                
                // 如果到达目标
                if (currentNode.Position == goal)
                {
                    return ReconstructPath(currentNode);
                }
                
                // 检查相邻节点
                foreach (var neighborPos in map.GetNeighbors(currentNode.Position))
                {
                    if (closedSet.Contains(neighborPos))
                        continue;
                    
                    var tile = map.GetTile(neighborPos);
                    if (tile == null || !tile.IsPassable)
                        continue;
                    
                    // 检查是否有其他单位占据
                    if (tile.OccupyingUnit != null && tile.OccupyingUnit != unit)
                        continue;
                    
                    // 计算移动代价
                    float movementCost = tile.MovementCost;
                    float tentativeG = currentNode.G + movementCost;
                    
                    Node neighborNode;
                    if (!allNodes.TryGetValue(neighborPos, out neighborNode))
                    {
                        neighborNode = new Node(neighborPos);
                        allNodes[neighborPos] = neighborNode;
                    }
                    
                    if (tentativeG < neighborNode.G || !openSet.Contains(neighborNode))
                    {
                        neighborNode.Parent = currentNode;
                        neighborNode.G = tentativeG;
                        neighborNode.H = Heuristic(neighborPos, goal);
                        
                        if (!openSet.Contains(neighborNode))
                        {
                            openSet.Add(neighborNode);
                        }
                    }
                }
            }
            
            // 没有找到路径
            return new List<Position>();
        }
        
        private static List<Position> ReconstructPath(Node node)
        {
            var path = new List<Position>();
            var current = node;
            
            while (current != null)
            {
                path.Add(current.Position);
                current = current.Parent;
            }
            
            path.Reverse();
            return path;
        }
        
        private static float Heuristic(Position a, Position b)
        {
            // 使用曼哈顿距离作为启发式函数
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
