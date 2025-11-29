using System.Collections.Generic;
using Code.Logic.Match3;

namespace Code.Services.Board
{
    public interface IMatchFinderService
    {
        List<GamePiece> FindMatch(GamePiece[,] pieces, GamePiece piece, int x, int y, BoardMatchConfig config);
    }
}

