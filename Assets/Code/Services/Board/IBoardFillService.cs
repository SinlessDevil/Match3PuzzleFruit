using System.Threading;
using Code.Logic.Match3;
using Cysharp.Threading.Tasks;

namespace Code.Services.Board
{
    public interface IBoardFillService
    {
        bool IsFilling { get; }
        
        UniTask FillAsync(GamePieceView[,] pieces, BoardFillConfig config, CancellationToken cancellationToken = default);
        
        bool FillStep(GamePieceView[,] pieces, BoardFillConfig config, ref bool inverse);
    }
}

