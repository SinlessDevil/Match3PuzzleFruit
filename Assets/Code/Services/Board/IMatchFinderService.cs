using System.Collections.Generic;
using Code.Logic.Match3;

namespace Code.Services.Board
{
    public interface IMatchFinderService
    {
        List<GamePieceView> FindMatch(GamePieceView[,] pieces, GamePieceView pieceView, int x, int y, BoardMatchConfig config);
    }
}

