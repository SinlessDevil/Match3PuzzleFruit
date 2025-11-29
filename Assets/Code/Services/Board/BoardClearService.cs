using System.Collections.Generic;
using Code.Logic.Match3;
using UnityEngine;

namespace Code.Services.Board
{
    public class BoardClearService : IBoardClearService
    {
        private readonly IMatchFinderService _matchFinderService;
        
        public BoardClearService(IMatchFinderService matchFinderService)
        {
            _matchFinderService = matchFinderService;
        }
        
        public bool ClearAllValidMatches(GamePieceView[,] pieces, BoardClearConfig config, 
            GamePieceView pressedPiece = null, GamePieceView enteredPiece = null)
        {
            bool needsRefill = false;
            BoardMatchConfig matchConfig = new BoardMatchConfig 
            { 
                XDim = config.XDim, 
                YDim = config.YDim 
            };

            for (int y = 0; y < config.YDim; y++)
            {
                for (int x = 0; x < config.XDim; x++)
                {
                    GamePieceView pieceView = pieces[x, y];
                    if (pieceView?.Data == null || !pieceView.Data.IsClearable()) 
                        continue;

                    List<GamePieceView> match = _matchFinderService.FindMatch(pieces, pieceView, x, y, matchConfig);

                    if (match == null || match.Count == 0) 
                        continue;

                    PieceType specialPieceType = PieceType.Count;
                    GamePieceView randomPiece = match[UnityEngine.Random.Range(0, match.Count)];
                    int specialPieceX = randomPiece.Data.X;
                    int specialPieceY = randomPiece.Data.Y;

                    if (match.Count == 4)
                    {
                        if (pressedPiece?.Data == null || enteredPiece?.Data == null)
                        {
                            specialPieceType = (PieceType)UnityEngine.Random.Range((int)PieceType.RowClear, (int)PieceType.ColumnClear);
                        }
                        else if (pressedPiece.Data.Y == enteredPiece.Data.Y)
                        {
                            specialPieceType = PieceType.RowClear;
                        }
                        else
                        {
                            specialPieceType = PieceType.ColumnClear;
                        }
                    }
                    else if (match.Count >= 5)
                    {
                        specialPieceType = PieceType.Rainbow;
                    }

                    foreach (GamePieceView gamePieceView in match)
                    {
                        if (gamePieceView?.Data == null || !ClearPiece(pieces, gamePieceView.Data.X, gamePieceView.Data.Y, config)) 
                            continue;

                        needsRefill = true;

                        if (gamePieceView != pressedPiece && gamePieceView != enteredPiece) 
                            continue;

                        specialPieceX = gamePieceView.Data.X;
                        specialPieceY = gamePieceView.Data.Y;
                    }

                    if (specialPieceType == PieceType.Count) 
                        continue;

                    pieces[specialPieceX, specialPieceY].Dispose();

                    GamePieceView newPieceView = SpawnNewPiece(pieces, specialPieceX, specialPieceY, specialPieceType, config);

                    if ((specialPieceType == PieceType.RowClear || specialPieceType == PieceType.ColumnClear) 
                        && newPieceView.Data.IsColored() && match[0].Data.IsColored())
                    {
                        newPieceView.ColorComponent.SetColor(match[0].ColorComponent.Color);
                    }
                    else if (specialPieceType == PieceType.Rainbow && newPieceView.Data.IsColored())
                    {
                        newPieceView.ColorComponent.SetColor(ColorType.Any);
                    }
                }
            }

            return needsRefill;
        }

        public bool ClearPiece(GamePieceView[,] pieces, int x, int y, BoardClearConfig config)
        {
            GamePieceView pieceView = pieces[x, y];
            if (pieceView?.Data == null || !pieceView.Data.IsClearable() || pieceView.ClearableComponent.IsBeingCleared) 
                return false;

            pieceView.ClearableComponent.Clear();
            SpawnNewPiece(pieces, x, y, PieceType.Empty, config);

            ClearObstacles(pieces, x, y, config);

            return true;
        }

        public void ClearRow(GamePieceView[,] pieces, int row, BoardClearConfig config)
        {
            for (int x = 0; x < config.XDim; x++)
            {
                ClearPiece(pieces, x, row, config);
            }
        }

        public void ClearColumn(GamePieceView[,] pieces, int column, BoardClearConfig config)
        {
            for (int y = 0; y < config.YDim; y++)
            {
                ClearPiece(pieces, column, y, config);
            }
        }

        public void ClearColor(GamePieceView[,] pieces, ColorType color, BoardClearConfig config)
        {
            for (int x = 0; x < config.XDim; x++)
            {
                for (int y = 0; y < config.YDim; y++)
                {
                    GamePieceView pieceView = pieces[x, y];
                    if (pieceView?.Data == null)
                        continue;
                        
                    if ((pieceView.Data.IsColored() && pieceView.ColorComponent.Color == color)
                        || (color == ColorType.Any))
                    {
                        ClearPiece(pieces, x, y, config);
                    }
                }
            }
        }

        private void ClearObstacles(GamePieceView[,] pieces, int x, int y, BoardClearConfig config)
        {
            for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
            {
                if (adjacentX == x || adjacentX < 0 || adjacentX >= config.XDim) 
                    continue;

                GamePieceView pieceView = pieces[adjacentX, y];
                if (pieceView?.Data == null || pieceView.Data.Type != PieceType.Bubble || !pieceView.Data.IsClearable()) 
                    continue;

                pieceView.ClearableComponent.Clear();
                SpawnNewPiece(pieces, adjacentX, y, PieceType.Empty, config);
            }

            for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
            {
                if (adjacentY == y || adjacentY < 0 || adjacentY >= config.YDim) 
                    continue;

                GamePieceView pieceView = pieces[x, adjacentY];
                if (pieceView?.Data == null || pieceView.Data.Type != PieceType.Bubble || !pieceView.Data.IsClearable()) 
                    continue;

                pieceView.ClearableComponent.Clear();
                SpawnNewPiece(pieces, x, adjacentY, PieceType.Empty, config);
            }
        }

        private GamePieceView SpawnNewPiece(GamePieceView[,] pieces, int x, int y, PieceType type, BoardClearConfig config)
        {
            Piece newPiece = config.PieceFactory.CreatePieceByCurrentLevel(type, 
                config.GetWorldPosition(x, y), Quaternion.identity, config.Root);
            GamePieceView pieceView = newPiece.GetComponent<GamePieceView>();
            
            GamePieceData data = new GamePieceData(x, y, type);
            data.SetMatchBoardController(config.MatchBoardController);
            pieceView.Initialize(data);
            
            pieces[x, y] = pieceView;

            return pieces[x, y];
        }
    }
}
