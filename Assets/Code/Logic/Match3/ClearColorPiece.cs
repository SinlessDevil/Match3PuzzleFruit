namespace Code.Logic.Match3
{
    public class ClearColorPiece : ClearablePiece
    {
        public ColorType Color { get; set; }

        public override void Clear()
        {
            base.Clear();

            if (_pieceView?.Data?.MatchBoardController != null)
            {
                _pieceView.Data.MatchBoardController.ClearColor(Color);
            }
        }
    }
}
