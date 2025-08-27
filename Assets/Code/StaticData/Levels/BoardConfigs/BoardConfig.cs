using System.Collections.Generic;
using Match3;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code.StaticData.Levels.BoardConfigs
{
    [CreateAssetMenu(fileName = "BoardConfig", menuName = "StaticData/Grid", order = 0)]
    public class BoardConfig : SerializedScriptableObject
    {
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
        public Dictionary<PieceType, GameObject> PieceDictionary;
        public PiecePosition[] InitialPieces;
    }
}