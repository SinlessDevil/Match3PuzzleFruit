using Code.Logic.Match3;

namespace Code.Services.Board
{
    public interface IBoardRandomService
    {
        ColorType GetRandomColorForCell(GamePieceView[,] pieces, int x, int y, int numColors);
    }
}


