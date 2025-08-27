using System.Collections.Generic;
using Match3;
using UnityEngine;

namespace Code.Logic.Controllers
{
    public interface IMatchBoardController
    {
        void StartLevel();
        void Dispose();
        void SetRootTransform(Transform rootTransform);
        bool IsFilling { get; }
        Vector2 GetWorldPosition(int x, int y);
        void PressPiece(GamePiece piece);
        void EnterPiece(GamePiece piece);
        void ReleasePiece();
        void ClearRow(int row);
        void ClearColumn(int column);
        void ClearColor(ColorType color);
        void GameOver();
        List<GamePiece> GetPiecesOfType(PieceType type);
    }
}