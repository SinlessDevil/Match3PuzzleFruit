using Match3;
using UnityEngine;

namespace Code.StaticData.Levels.BoardConfigs
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "StaticData/Grid", order = 0)]
    public class BoardConfig : ScriptableObject
    {
        [System.Serializable]
        public struct PiecePrefab
        {
            public PieceType Type;
            public GameObject Prefab;
        };

        [System.Serializable]
        public struct PiecePosition
        {
            public PieceType Type;
            public int X;
            public int Y;
        };

        public int XDim;
        public int YDim;
        public float FillTime;

        public PiecePrefab[] PiecePrefabs;
        public GameObject BackgroundPrefab;

        public PiecePosition[] InitialPieces;
    }
}