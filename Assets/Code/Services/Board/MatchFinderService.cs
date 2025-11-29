using System.Collections.Generic;
using Code.Logic.Match3;

namespace Code.Services.Board
{
    public class MatchFinderService : IMatchFinderService
    {
        public List<GamePiece> FindMatch(GamePiece[,] pieces, GamePiece piece, int newX, int newY, BoardMatchConfig config)
        {
            if (!piece.IsColored()) 
                return null;
                
            ColorType color = piece.ColorComponent.Color;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < config.XDim; xOffset++)
                {
                    int x = dir == 0 ? newX - xOffset : newX + xOffset;

                    if (x < 0 || x >= config.XDim) 
                        break;

                    if (pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
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

                            if (pieces[horizontalPieces[i].X, y].IsColored() && 
                                pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
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
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < config.XDim; yOffset++)
                {
                    int y = dir == 0 ? newY - yOffset : newY + yOffset;

                    if (y < 0 || y >= config.YDim) 
                        break;

                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
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

                            if (pieces[x, verticalPieces[i].Y].IsColored() && 
                                pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
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

