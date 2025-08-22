using Code.Logic.Controllers;
using UnityEngine;
using Zenject;

namespace Match3
{
    public class GamePiece : MonoBehaviour
    {
        public int score;

        private int _x;
        private int _y;

        public int X
        {
            get => _x;
            set { if (IsMovable()) { _x = value; } }
        }

        public int Y
        {
            get => _y;
            set { if (IsMovable()) { _y = value; } }
        }
    
        private PieceType _type;

        public PieceType Type => _type;

        private MatchBoardController _matchBoardController;

        public MatchBoardController MatchBoardControllerRef => _matchBoardController;

        private MovablePiece _movableComponent;

        public MovablePiece MovableComponent => _movableComponent;

        private ColorPiece _colorComponent;

        public ColorPiece ColorComponent => _colorComponent;

        private ClearablePiece _clearableComponent;

        public ClearablePiece ClearableComponent => _clearableComponent;

        private void Awake()
        {
            _movableComponent = GetComponent<MovablePiece>();
            _colorComponent = GetComponent<ColorPiece>();
            _clearableComponent = GetComponent<ClearablePiece>();
        }

        public void Init(int x, int y, MatchBoardController matchBoardController, PieceType type)
        {
            _x = x;
            _y = y;
            _matchBoardController = matchBoardController;
            _type = type;
        }

        private void OnMouseEnter() => _matchBoardController.EnterPiece(this);

        private void OnMouseDown() => _matchBoardController.PressPiece(this);

        private void OnMouseUp() => _matchBoardController.ReleasePiece();

        public bool IsMovable() => _movableComponent != null;

        public bool IsColored() => _colorComponent != null;

        public bool IsClearable() => _clearableComponent != null;

        public void Dispose()
        {
            Destroy(this.gameObject);
        }
    }
}
