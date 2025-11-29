using Code.Logic.Match3;

namespace Code.Services.Board
{
    public interface IPieceSwapService
    {
        bool TrySwapPieces(GamePieceView[,] pieces, GamePieceView piece1, GamePieceView piece2, BoardSwapConfig config);
    }
}

