namespace Code.Logic.Match3
{
    public class ClearColorPiece : ClearablePiece
    {
        public ColorType Color { get; set; }

        public override void Clear()
        {
            base.Clear();

            _piece.MatchBoardControllerRef.ClearColor(Color);
        }
    }
}
