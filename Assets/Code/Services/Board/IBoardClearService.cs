using Code.Logic.Match3;

namespace Code.Services.Board
{
    public interface IBoardClearService
    {
        bool ClearAllValidMatches(GamePiece[,] pieces, BoardClearConfig config, 
            GamePiece pressedPiece = null, GamePiece enteredPiece = null);
        
        bool ClearPiece(GamePiece[,] pieces, int x, int y, BoardClearConfig config);
        
        void ClearRow(GamePiece[,] pieces, int row, BoardClearConfig config);
        
        void ClearColumn(GamePiece[,] pieces, int column, BoardClearConfig config);
        
        void ClearColor(GamePiece[,] pieces, ColorType color, BoardClearConfig config);
    }
}

