using System.Threading;
using Code.Logic.Match3;
using Cysharp.Threading.Tasks;

namespace Code.Services.Board
{
    public interface IBoardFillService
    {
        bool IsFilling { get; }
        
        UniTask FillAsync(GamePiece[,] pieces, BoardFillConfig config, CancellationToken cancellationToken = default);
        
        bool FillStep(GamePiece[,] pieces, BoardFillConfig config, ref bool inverse);
    }
}

