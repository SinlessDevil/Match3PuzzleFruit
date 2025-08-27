using Code.Logic.Match3;
using Match3;
using UnityEngine;

namespace Code.Services.Factories.Pieces
{
    public interface IPieceFactory
    {
        public Piece CreatePieceByCurrentLevel(PieceType pieceType, Vector3 position, Quaternion rotation, 
            Transform parent);
    }
}