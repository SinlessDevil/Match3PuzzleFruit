namespace Match3
{
    internal class ClearLinePiece : ClearablePiece
    {
        public bool isRow;

        public override void Clear()
        {
            base.Clear();

            if (isRow)
            {            
                _piece.MatchBoardControllerRef.ClearRow(_piece.Y);
            }
            else
            {            
                _piece.MatchBoardControllerRef.ClearColumn(_piece.X);
            }
        }
    }
}
