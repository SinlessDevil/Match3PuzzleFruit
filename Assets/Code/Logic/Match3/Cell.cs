using Code.Logic.Controllers;
using UnityEngine;

namespace Code.Logic.Match3
{
    public class Cell : MonoBehaviour
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        
        public GamePieceView CurrentPieceView { get; set; }
        
        public bool HasPieceView => CurrentPieceView != null;
        
        private MatchBoardController _matchBoardController;
        
        public void Initialize(int x, int y, MatchBoardController controller)
        {
            X = x;
            Y = y;
            _matchBoardController = controller;
        }
        
        private void OnMouseDown()
        {
            if (HasPieceView && _matchBoardController != null)
            {
                _matchBoardController.PressCell(this);
            }
        }
        
        private void OnMouseEnter()
        {
            if (Input.GetMouseButton(0) && HasPieceView && _matchBoardController != null)
            {
                _matchBoardController.EnterCell(this);
            }
        }
        
        private void OnMouseUp()
        {
            if (_matchBoardController != null)
            {
                _matchBoardController.ReleaseCell();
            }
        }
    }
}

