using System;
using System.Threading;
using Code.Logic.Controllers;
using Code.Logic.Match3;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Services.Board
{
    public class BoardFillService : IBoardFillService
    {
        private readonly IBoardClearService _boardClearService;
        
        public bool IsFilling { get; private set; }
        
        public BoardFillService(IBoardClearService boardClearService)
        {
            _boardClearService = boardClearService;
        }
        
        public async UniTask FillAsync(GamePiece[,] pieces, BoardFillConfig config, 
            CancellationToken cancellationToken = default)
        {
            bool needsRefill = true;
            IsFilling = true;
            bool inverse = false;

            while (needsRefill && !cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(config.FillTime), cancellationToken: cancellationToken);
                
                while (FillStep(pieces, config, ref inverse) && !cancellationToken.IsCancellationRequested)
                {
                    inverse = !inverse;
                    await UniTask.Delay(TimeSpan.FromSeconds(config.FillTime), cancellationToken: cancellationToken);
                }

                if (cancellationToken.IsCancellationRequested)
                    break;

                BoardClearConfig clearConfig = new BoardClearConfig
                {
                    XDim = config.XDim,
                    YDim = config.YDim,
                    Root = config.Root,
                    PieceFactory = config.PieceFactory,
                    MatchBoardController = config.MatchBoardController,
                    GetWorldPosition = config.GetWorldPosition
                };
                
                needsRefill = _boardClearService.ClearAllValidMatches(pieces, clearConfig);
            }

            IsFilling = false;
        }

        public bool FillStep(GamePiece[,] pieces, BoardFillConfig config, ref bool inverse)
        {
            bool movedPiece = false;

            for (int y = config.YDim - 2; y >= 0; y--)
            {
                for (int loopX = 0; loopX < config.XDim; loopX++)
                {
                    int x = inverse ? config.XDim - 1 - loopX : loopX;
                    GamePiece piece = pieces[x, y];

                    if (!piece.IsMovable()) 
                        continue;

                    GamePiece pieceBelow = pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.Empty)
                    {
                        pieceBelow.Dispose();
                        Vector2 endPosition = config.GetWorldPosition(x, y + 1);
                        piece.MovableComponent.Move(x, y + 1, endPosition, config.FillTime);
                        pieces[x, y + 1] = piece;
                        if (config.MatchBoardController != null)
                        {
                            piece.SetMatchBoardController(config.MatchBoardController);
                        }
                        SpawnNewPiece(pieces, x, y, PieceType.Empty, config);
                        movedPiece = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag == 0) 
                                continue;

                            int diagX = inverse ? x - diag : x + diag;

                            if (diagX < 0 || diagX >= config.XDim) 
                                continue;

                            GamePiece diagonalPiece = pieces[diagX, y + 1];

                            if (diagonalPiece.Type != PieceType.Empty) 
                                continue;

                            bool hasPieceAbove = true;

                            for (int aboveY = y; aboveY >= 0; aboveY--)
                            {
                                GamePiece pieceAbove = pieces[diagX, aboveY];

                                if (pieceAbove.IsMovable())
                                {
                                    break;
                                }
                                else if (pieceAbove.Type != PieceType.Empty)
                                {
                                    hasPieceAbove = false;
                                    break;
                                }
                            }

                            if (hasPieceAbove) 
                                continue;

                            diagonalPiece.Dispose();
                            Vector2 endPosition = config.GetWorldPosition(diagX, y + 1);
                            piece.MovableComponent.Move(diagX, y + 1, endPosition, config.FillTime);
                            pieces[diagX, y + 1] = piece;
                            if (config.MatchBoardController != null)
                            {
                                piece.SetMatchBoardController(config.MatchBoardController);
                            }
                            SpawnNewPiece(pieces, x, y, PieceType.Empty, config);
                            movedPiece = true;
                            break;
                        }
                    }
                }
            }

            for (int x = 0; x < config.XDim; x++)
            {
                GamePiece pieceBelow = pieces[x, 0];

                if (pieceBelow.Type != PieceType.Empty) 
                    continue;

                pieceBelow.Dispose();
                Piece newPiece = config.PieceFactory.CreatePieceByCurrentLevel(PieceType.Normal, 
                    config.GetWorldPosition(x, -1), Quaternion.identity, config.Root);

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, null, PieceType.Normal);
                Vector2 endPosition = config.GetWorldPosition(x, 0);
                pieces[x, 0].MovableComponent.Move(x, 0, endPosition, config.FillTime);
                pieces[x, 0].ColorComponent.SetColor((ColorType)UnityEngine.Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                movedPiece = true;
            }

            return movedPiece;
        }

        private GamePiece SpawnNewPiece(GamePiece[,] pieces, int x, int y, PieceType type, BoardFillConfig config)
        {
            Piece newPiece = config.PieceFactory.CreatePieceByCurrentLevel(type, 
                config.GetWorldPosition(x, y), Quaternion.identity, config.Root);
            pieces[x, y] = newPiece.GetComponent<GamePiece>();
            pieces[x, y].Init(x, y, config.MatchBoardController, type);

            return pieces[x, y];
        }
    }
}

