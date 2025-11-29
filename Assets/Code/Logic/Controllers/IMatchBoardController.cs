using System.Collections.Generic;
using Code.Logic.Match3;
using UnityEngine;

namespace Code.Logic.Controllers
{
    public interface IMatchBoardController
    {
        public void StartLevel();
        public void Dispose();
        public void SetRootTransform(Transform rootTransform);
        public bool IsFilling { get; }
        public Vector2 GetWorldPosition(int x, int y);
        public void PressPiece(GamePieceView pieceView);
        public void EnterPiece(GamePieceView pieceView);
        public void ReleasePiece();
        public void ClearRow(int row);
        public void ClearColumn(int column);
        public void ClearColor(ColorType color);
        public void GameOver();
        public List<GamePieceView> GetPiecesOfType(PieceType type);
    }
}