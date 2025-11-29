using Code.Logic.Match3;
using Code.Services.Factories.Pieces;
using Code.Services.LevelConductors.Locator;
using UnityEngine;

namespace Code.Services.Board
{
    public class BoardSwapConfig
    {
        public int XDim { get; set; }
        public int YDim { get; set; }
        public float FillTime { get; set; }
        public Transform Root { get; set; }
        public IPieceFactory PieceFactory { get; set; }
        public System.Action OnMoveCallback { get; set; }
        public System.Func<int, int, Vector2> GetWorldPosition { get; set; }
    }
}

