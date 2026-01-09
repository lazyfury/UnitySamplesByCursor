using Match3Engine.Core.Interfaces;
using Match3Engine.Core.Types;

namespace Match3Engine.Core.Implementations
{
    /// <summary>
    /// 宝石实现
    /// </summary>
    public class Gem : IGem
    {
        private static int _nextId = 1;
        
        public int Id { get; private set; }
        public GemType Type { get; set; }
        public BoardPosition Position { get; set; }
        public bool IsSpecial => Type == GemType.Special;
        
        public Gem(GemType type, BoardPosition position)
        {
            Id = _nextId++;
            Type = type;
            Position = position;
        }
        
        public Gem(GemType type, int row, int col)
        {
            Id = _nextId++;
            Type = type;
            Position = new BoardPosition(row, col);
        }
    }
}
