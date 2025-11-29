using Code.Logic.Controllers;
using Code.Logic.Match3;
using Code.Services.Factories.Pieces;
using UnityEngine;

namespace Code.Services.Board
{
    public class BoardClearConfig
    {
        public int XDim { get; set; }
        public int YDim { get; set; }
        public Transform Root { get; set; }
        public IPieceFactory PieceFactory { get; set; }
        public MatchBoardController MatchBoardController { get; set; }
        public System.Func<int, int, Vector2> GetWorldPosition { get; set; }
    }
}

