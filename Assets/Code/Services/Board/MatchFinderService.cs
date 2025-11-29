using System.Collections.Generic;
using Code.Logic.Match3;

namespace Code.Services.Board
{
    public class MatchFinderService : IMatchFinderService
    {
        public List<GamePieceView> FindMatch(GamePieceView[,] pieces, GamePieceView pieceView, int newX, int newY, BoardMatchConfig config)
        {
            if (pieceView?.Data == null || !pieceView.Data.IsColored()) 
                return null;
                
            ColorType color = pieceView.ColorComponent.Color;
            List<GamePieceView> horizontalPieces = new List<GamePieceView>();
            List<GamePieceView> verticalPieces = new List<GamePieceView>();
            List<GamePieceView> matchingPieces = new List<GamePieceView>();

            horizontalPieces.Add(pieceView);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < config.XDim; xOffset++)
                {
                    int x = dir == 0 ? newX - xOffset : newX + xOffset;

                    if (x < 0 || x >= config.XDim) 
                        break;

                    GamePieceView currentPiece = pieces[x, newY];
                    if (currentPiece?.Data != null && currentPiece.Data.IsColored() && currentPiece.ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(currentPiece);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (horizontalPieces.Count >= 3)
            {
                matchingPieces.AddRange(horizontalPieces);
            }

            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < config.YDim; yOffset++)
                        {
                            int y = dir == 0 ? newY - yOffset : newY + yOffset;

                            if (y < 0 || y >= config.YDim)
                                break;

                            GamePieceView currentPiece = pieces[horizontalPieces[i].Data.X, y];
                            if (currentPiece?.Data != null && currentPiece.Data.IsColored() && 
                                currentPiece.ColorComponent.Color == color)
                            {
                                verticalPieces.Add(currentPiece);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        matchingPieces.AddRange(verticalPieces);
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(pieceView);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < config.XDim; yOffset++)
                {
                    int y = dir == 0 ? newY - yOffset : newY + yOffset;

                    if (y < 0 || y >= config.YDim) 
                        break;

                    GamePieceView currentPiece = pieces[newX, y];
                    if (currentPiece?.Data != null && currentPiece.Data.IsColored() && currentPiece.ColorComponent.Color == color)
                    {
                        verticalPieces.Add(currentPiece);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                matchingPieces.AddRange(verticalPieces);
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < config.YDim; xOffset++)
                        {
                            int x = dir == 0 ? newX - xOffset : newX + xOffset;

                            if (x < 0 || x >= config.XDim)
                                break;

                            GamePieceView currentPiece = pieces[x, verticalPieces[i].Data.Y];
                            if (currentPiece?.Data != null && currentPiece.Data.IsColored() && 
                                currentPiece.ColorComponent.Color == color)
                            {
                                horizontalPieces.Add(currentPiece);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        matchingPieces.AddRange(horizontalPieces);
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            return null;
        }
    }
}
