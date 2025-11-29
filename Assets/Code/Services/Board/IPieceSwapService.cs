using Code.Logic.Match3;

namespace Code.Services.Board
{
    public interface IPieceSwapService
    {
        bool TrySwapPieces(GamePiece[,] pieces, GamePiece piece1, GamePiece piece2, BoardSwapConfig config);
    }
}

