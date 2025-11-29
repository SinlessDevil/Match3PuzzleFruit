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
        
        public bool TrySwapPieces(GamePiece[,] pieces, GamePiece piece1, GamePiece piece2, BoardSwapConfig config)
        {
            if (!piece1.IsMovable() || !piece2.IsMovable()) 
                return false;

            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            BoardMatchConfig matchConfig = new BoardMatchConfig 
            { 
                XDim = pieces.GetLength(0), 
                YDim = pieces.GetLength(1) 
            };

            bool hasMatch = _matchFinderService.FindMatch(pieces, piece1, piece2.X, piece2.Y, matchConfig) != null ||
                           _matchFinderService.FindMatch(pieces, piece2, piece1.X, piece1.Y, matchConfig) != null ||
                           piece1.Type == PieceType.Rainbow ||
                           piece2.Type == PieceType.Rainbow;

            if (hasMatch)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                Vector2 piece1EndPosition = config.GetWorldPosition(piece2.X, piece2.Y);
                Vector2 piece2EndPosition = config.GetWorldPosition(piece1X, piece1Y);
                
                piece1.MovableComponent.Move(piece2.X, piece2.Y, piece1EndPosition, config.FillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, piece2EndPosition, config.FillTime);

                if (piece1.Type == PieceType.Rainbow && piece1.IsClearable() && piece2.IsColored())
                {
                    ClearColorPiece clearColor = piece1.GetComponent<ClearColorPiece>();

                    if (clearColor != null)
                    {
                        clearColor.Color = piece2.ColorComponent.Color;
                    }

                    BoardClearConfig clearConfig = CreateClearConfig(config);
                    _boardClearService.ClearPiece(pieces, piece1.X, piece1.Y, clearConfig);
                }

                if (piece2.Type == PieceType.Rainbow && piece2.IsClearable() && piece1.IsColored())
                {
                    ClearColorPiece clearColor = piece2.GetComponent<ClearColorPiece>();

                    if (clearColor != null)
                    {
                        clearColor.Color = piece1.ColorComponent.Color;
                    }

                    BoardClearConfig clearConfig = CreateClearConfig(config);
                    _boardClearService.ClearPiece(pieces, piece2.X, piece2.Y, clearConfig);
                }

                BoardClearConfig clearConfigForMatches = CreateClearConfig(config);
                _boardClearService.ClearAllValidMatches(pieces, clearConfigForMatches, piece1, piece2);

                if (piece1.Type == PieceType.RowClear || piece1.Type == PieceType.ColumnClear)
                {
                    BoardClearConfig clearConfig = CreateClearConfig(config);
                    _boardClearService.ClearPiece(pieces, piece1.X, piece1.Y, clearConfig);
                }

                if (piece2.Type == PieceType.RowClear || piece2.Type == PieceType.ColumnClear)
                {
                    BoardClearConfig clearConfig = CreateClearConfig(config);
                    _boardClearService.ClearPiece(pieces, piece2.X, piece2.Y, clearConfig);
                }

                config.OnMoveCallback?.Invoke();

                return true;
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
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
                GetWorldPosition = config.GetWorldPosition
            };
        }
    }
}

