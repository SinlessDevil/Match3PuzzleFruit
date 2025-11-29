using System.Collections.Generic;
using Code.Logic.Controllers;
using Code.Logic.Match3;
using Code.Services.Factories.Pieces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Code.Services.Board
{
    public class BoardClearService : IBoardClearService
    {
        private readonly IMatchFinderService _matchFinderService;
        
        public BoardClearService(IMatchFinderService matchFinderService)
        {
            _matchFinderService = matchFinderService;
        }
        
        public bool ClearAllValidMatches(GamePiece[,] pieces, BoardClearConfig config, 
            GamePiece pressedPiece = null, GamePiece enteredPiece = null)
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
                    if (!pieces[x, y].IsClearable()) 
                        continue;

                    List<GamePiece> match = _matchFinderService.FindMatch(pieces, pieces[x, y], x, y, matchConfig);

                    if (match == null) 
                        continue;

                    PieceType specialPieceType = PieceType.Count;
                    GamePiece randomPiece = match[UnityEngine.Random.Range(0, match.Count)];
                    int specialPieceX = randomPiece.X;
                    int specialPieceY = randomPiece.Y;

                    if (match.Count == 4)
                    {
                        if (pressedPiece == null || enteredPiece == null)
                        {
                            specialPieceType = (PieceType)UnityEngine.Random.Range((int)PieceType.RowClear, (int)PieceType.ColumnClear);
                        }
                        else if (pressedPiece.Y == enteredPiece.Y)
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

                    foreach (GamePiece gamePiece in match)
                    {
                        if (!ClearPiece(pieces, gamePiece.X, gamePiece.Y, config)) 
                            continue;

                        needsRefill = true;

                        if (gamePiece != pressedPiece && gamePiece != enteredPiece) 
                            continue;

                        specialPieceX = gamePiece.X;
                        specialPieceY = gamePiece.Y;
                    }

                    if (specialPieceType == PieceType.Count) 
                        continue;

                    pieces[specialPieceX, specialPieceY].Dispose();

                    GamePiece newPiece = SpawnNewPiece(pieces, specialPieceX, specialPieceY, specialPieceType, config);

                    if ((specialPieceType == PieceType.RowClear || specialPieceType == PieceType.ColumnClear) 
                        && newPiece.IsColored() && match[0].IsColored())
                    {
                        newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                    }
                    else if (specialPieceType == PieceType.Rainbow && newPiece.IsColored())
                    {
                        newPiece.ColorComponent.SetColor(ColorType.Any);
                    }
                }
            }

            return needsRefill;
        }

        public bool ClearPiece(GamePiece[,] pieces, int x, int y, BoardClearConfig config)
        {
            if (!pieces[x, y].IsClearable() || pieces[x, y].ClearableComponent.IsBeingCleared) 
                return false;

            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(pieces, x, y, PieceType.Empty, config);

            ClearObstacles(pieces, x, y, config);

            return true;
        }

        public void ClearRow(GamePiece[,] pieces, int row, BoardClearConfig config)
        {
            for (int x = 0; x < config.XDim; x++)
            {
                ClearPiece(pieces, x, row, config);
            }
        }

        public void ClearColumn(GamePiece[,] pieces, int column, BoardClearConfig config)
        {
            for (int y = 0; y < config.YDim; y++)
            {
                ClearPiece(pieces, column, y, config);
            }
        }

        public void ClearColor(GamePiece[,] pieces, ColorType color, BoardClearConfig config)
        {
            for (int x = 0; x < config.XDim; x++)
            {
                for (int y = 0; y < config.YDim; y++)
                {
                    if ((pieces[x, y].IsColored() && pieces[x, y].ColorComponent.Color == color)
                        || (color == ColorType.Any))
                    {
                        ClearPiece(pieces, x, y, config);
                    }
                }
            }
        }

        private void ClearObstacles(GamePiece[,] pieces, int x, int y, BoardClearConfig config)
        {
            for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
            {
                if (adjacentX == x || adjacentX < 0 || adjacentX >= config.XDim) 
                    continue;

                if (pieces[adjacentX, y].Type != PieceType.Bubble || !pieces[adjacentX, y].IsClearable()) 
                    continue;

                pieces[adjacentX, y].ClearableComponent.Clear();
                SpawnNewPiece(pieces, adjacentX, y, PieceType.Empty, config);
            }

            for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
            {
                if (adjacentY == y || adjacentY < 0 || adjacentY >= config.YDim) 
                    continue;

                if (pieces[x, adjacentY].Type != PieceType.Bubble || !pieces[x, adjacentY].IsClearable()) 
                    continue;

                pieces[x, adjacentY].ClearableComponent.Clear();
                SpawnNewPiece(pieces, x, adjacentY, PieceType.Empty, config);
            }
        }

        private GamePiece SpawnNewPiece(GamePiece[,] pieces, int x, int y, PieceType type, BoardClearConfig config)
        {
            Piece newPiece = config.PieceFactory.CreatePieceByCurrentLevel(type, 
                config.GetWorldPosition(x, y), Quaternion.identity, config.Root);
            pieces[x, y] = newPiece.GetComponent<GamePiece>();
            pieces[x, y].Init(x, y, config.MatchBoardController, type);

            return pieces[x, y];
        }
    }
}

