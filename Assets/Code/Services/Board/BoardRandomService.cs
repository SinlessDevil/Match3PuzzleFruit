using Code.Logic.Match3;
using UnityEngine;

namespace Code.Services.Board
{
    public class BoardRandomService : IBoardRandomService
    {
        public ColorType GetRandomColorForCell(GamePieceView[,] pieces, int x, int y, int numColors)
        {
            if (numColors <= 0)
            {
                return ColorType.Yellow;
            }

            int maxAttempts = 10;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                int colorIndex = UnityEngine.Random.Range(0, numColors);
                ColorType candidate = (ColorType)colorIndex;

                if (WouldCreateMatch(pieces, x, y, candidate))
                {
                    continue;
                }

                return candidate;
            }

            int fallbackIndex = UnityEngine.Random.Range(0, numColors);
            return (ColorType)fallbackIndex;
        }

        private bool WouldCreateMatch(GamePieceView[,] pieces, int x, int y, ColorType color)
        {
            if (pieces == null)
            {
                return false;
            }

            int xDim = pieces.GetLength(0);
            int yDim = pieces.GetLength(1);

            if (x < 0 || x >= xDim || y < 0 || y >= yDim)
            {
                return false;
            }

            // Проверка по горизонтали: две одинаковые слева.
            if (x >= 2)
            {
                ColorType left1 = GetColor(pieces[x - 1, y]);
                ColorType left2 = GetColor(pieces[x - 2, y]);

                if (left1 == color && left2 == color)
                {
                    return true;
                }
            }

            // Проверка по вертикали: две одинаковые снизу (на уже заспавненных клетках).
            if (y >= 2)
            {
                ColorType down1 = GetColor(pieces[x, y - 1]);
                ColorType down2 = GetColor(pieces[x, y - 2]);

                if (down1 == color && down2 == color)
                {
                    return true;
                }
            }

            return false;
        }

        private ColorType GetColor(GamePieceView view)
        {
            if (view == null || view.ColorComponent == null)
            {
                return ColorType.Count;
            }

            return view.ColorComponent.Color;
        }
    }
}


