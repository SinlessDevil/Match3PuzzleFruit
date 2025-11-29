using Code.Logic.Controllers;

namespace Code.Logic.Match3
{
    public class GamePieceData
    {
        public int Score { get; set; }
        
        public int X { get; private set; }
        public int Y { get; private set; }
        
        public PieceType Type { get; private set; }
        
        public MatchBoardController MatchBoardController { get; private set; }
        
        public bool HasMovableComponent { get; set; }
        public bool HasColorComponent { get; set; }
        public bool HasClearableComponent { get; set; }
        
        public GamePieceData(int x, int y, PieceType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
        
        public void SetPosition(int x, int y)
        {
            if (HasMovableComponent)
            {
                X = x;
                Y = y;
            }
        }
        
        public void SetMatchBoardController(MatchBoardController matchBoardController)
        {
            MatchBoardController = matchBoardController;
        }
        
        public bool IsMovable() => HasMovableComponent;
        public bool IsColored() => HasColorComponent;
        public bool IsClearable() => HasClearableComponent;
    }
}

