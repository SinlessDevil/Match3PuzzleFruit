using Code.Logic.Match3;

namespace Code.Services.Board
{
    public interface IBoardClearService
    {
        bool ClearAllValidMatches(GamePieceView[,] pieces, BoardClearConfig config, 
            GamePieceView pressedPiece = null, GamePieceView enteredPiece = null);
        
        bool ClearPiece(GamePieceView[,] pieces, int x, int y, BoardClearConfig config);
        
        void ClearRow(GamePieceView[,] pieces, int row, BoardClearConfig config);
        
        void ClearColumn(GamePieceView[,] pieces, int column, BoardClearConfig config);
        
        void ClearColor(GamePieceView[,] pieces, ColorType color, BoardClearConfig config);
    }
}

