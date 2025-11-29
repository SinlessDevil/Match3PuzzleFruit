using Code.Logic.Match3;
using UnityEngine;

namespace Code.Services.Board
{
    public class PieceSwapService : IPieceSwapService
    {
        private readonly IMatchFinderService _matchFinderService;
        private readonly IBoardClearService _boardClearService;
        
        public PieceSwapService(IMatchFinderService matchFinderService, IBoardClearService boardClearService)
        {
            _matchFinderService = matchFinderService;
            _boardClearService = boardClearService;
        }
        
        public bool TrySwapPieces(GamePieceView[,] pieces, GamePieceView piece1, GamePieceView piece2, BoardSwapConfig config)
        {
            if (piece1?.Data == null || piece2?.Data == null || 
                !piece1.Data.IsMovable() || !piece2.Data.IsMovable()) 
                return false;

            pieces[piece1.Data.X, piece1.Data.Y] = piece2;
            pieces[piece2.Data.X, piece2.Data.Y] = piece1;

            BoardMatchConfig matchConfig = new BoardMatchConfig 
            { 
                XDim = pieces.GetLength(0), 
                YDim = pieces.GetLength(1) 
            };

            bool hasMatch = _matchFinderService.FindMatch(pieces, piece1, piece2.Data.X, piece2.Data.Y, matchConfig) != null ||
                           _matchFinderService.FindMatch(pieces, piece2, piece1.Data.X, piece1.Data.Y, matchConfig) != null ||
                           piece1.Data.Type == PieceType.Rainbow ||
                           piece2.Data.Type == PieceType.Rainbow;

            if (hasMatch)
            {
                int piece1X = piece1.Data.X;
                int piece1Y = piece1.Data.Y;

                Vector2 piece1EndPosition = config.GetWorldPosition(piece2.Data.X, piece2.Data.Y);
                Vector2 piece2EndPosition = config.GetWorldPosition(piece1X, piece1Y);
                
                piece1.MovableComponent.Move(piece2.Data.X, piece2.Data.Y, piece1EndPosition, config.FillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, piece2EndPosition, config.FillTime);

                if (piece1.Data.Type == PieceType.Rainbow && piece1.Data.IsClearable() && piece1.Data.IsColored())
                {
                    ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

                    if (clearColor != null && piece2.Data.IsColored())
                    {
                        clearColor.Color = piece2.ColorComponent.Color;
                    }

                    BoardClearConfig clearConfig = CreateClearConfig(config);
                    _boardClearService.ClearPiece(pieces, piece1.Data.X, piece1.Data.Y, clearConfig);
                }

                if (piece2.Data.Type == PieceType.Rainbow && piece2.Data.IsClearable() && piece2.Data.IsColored())
                {
                    ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();

                    if (clearColor != null && piece1.Data.IsColored())
                    {
                        clearColor.Color = piece1.ColorComponent.Color;
                    }

                    BoardClearConfig clearConfig = CreateClearConfig(config);
                    _boardClearService.ClearPiece(pieces, piece2.Data.X, piece2.Data.Y, clearConfig);
                }

                BoardClearConfig clearConfigForMatches = CreateClearConfig(config);
                _boardClearService.ClearAllValidMatches(pieces, clearConfigForMatches, piece1, piece2);

                if (piece1.Data.Type == PieceType.RowClear || piece1.Data.Type == PieceType.ColumnClear)
                {
                    BoardClearConfig clearConfig = CreateClearConfig(config);
                    _boardClearService.ClearPiece(pieces, piece1.Data.X, piece1.Data.Y, clearConfig);
                }

                if (piece2.Data.Type == PieceType.RowClear || piece2.Data.Type == PieceType.ColumnClear)
                {
                    BoardClearConfig clearConfig = CreateClearConfig(config);
                    _boardClearService.ClearPiece(pieces, piece2.Data.X, piece2.Data.Y, clearConfig);
                }

                config.OnMoveCallback?.Invoke();

                return true;
            }
            else
            {
                pieces[piece1.Data.X, piece1.Data.Y] = piece1;
                pieces[piece2.Data.X, piece2.Data.Y] = piece2;
                return false;
            }
        }

        private BoardClearConfig CreateClearConfig(BoardSwapConfig config)
        {
            return new BoardClearConfig
            {
                XDim = config.XDim,
                YDim = config.YDim,
                Root = config.Root,
                PieceFactory = config.PieceFactory,
                MatchBoardController = null,
                GetWorldPosition = config.GetWorldPosition
            };
        }
    }
}
