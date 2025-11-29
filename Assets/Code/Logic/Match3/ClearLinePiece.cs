namespace Code.Logic.Match3
{
    internal class ClearLinePiece : ClearablePiece
    {
        public bool isRow;

        public override void Clear()
        {
            base.Clear();

            if (_pieceView?.Data?.MatchBoardController == null)
                return;

            if (isRow)
            {            
                _pieceView.Data.MatchBoardController.ClearRow(_pieceView.Data.Y);
            }
            else
            {            
                _pieceView.Data.MatchBoardController.ClearColumn(_pieceView.Data.X);
            }
        }
    }
}
