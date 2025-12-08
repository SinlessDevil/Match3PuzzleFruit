using System;
using Code.Logic.Controllers;
using UnityEngine;

namespace Code.Logic.Match3
{
    public class GamePieceView : MonoBehaviour
    {
        [SerializeField] private MovablePieceView _movableComponent;
        [SerializeField] private ColorPiece _colorComponent;
        [SerializeField] private ClearablePiece _clearableComponent;
     
        private GamePieceData _data;
        
        private void OnValidate()
        {
            if (_movableComponent == null)
            {
                _movableComponent = GetComponent<MovablePieceView>();
            }

            if (_colorComponent == null)
            {
                _colorComponent = GetComponent<ColorPiece>();
            }

            if (_clearableComponent == null)
            {
                _clearableComponent = GetComponent<ClearablePiece>();
            }
        }
        
        public GamePieceData Data => _data;
        public MovablePieceView MovableComponent => _movableComponent;
        public ColorPiece ColorComponent => _colorComponent;
        public ClearablePiece ClearableComponent => _clearableComponent;

        public event Action<GamePieceView> OnPiecePressed;
        public event Action<GamePieceView> OnPieceEntered;
        public event Action<GamePieceView> OnPieceReleased;

        
        public int score
        {
            get => _data?.Score ?? 0;
            set
            {
                if (_data != null)
                {
                    _data.Score = value;
                }
            }
        }
        
        public void Initialize(GamePieceData data)
        {
            _data = data;
            
            if (_data != null)
            {
                _data.HasMovableComponent = _movableComponent != null;
                _data.HasColorComponent = _colorComponent != null;
                _data.HasClearableComponent = _clearableComponent != null;
                
                if (_data.MatchBoardController != null)
                {
                    SubscribeToController(_data.MatchBoardController);
                }
            }
        }
        
        public void SubscribeToController(MatchBoardController controller)
        {
            if (controller == null)
                return;
                
            OnPiecePressed += controller.PressPiece;
            OnPieceEntered += controller.EnterPiece;
            OnPieceReleased += (view) => controller.ReleasePiece();
        }
        
        private void OnMouseDown()
        {
            if (_data?.MatchBoardController != null)
            {
                OnPiecePressed?.Invoke(this);
            }
        }
        
        private void OnMouseEnter()
        {
            if (Input.GetMouseButton(0) && _data?.MatchBoardController != null)
            {
                OnPieceEntered?.Invoke(this);
            }
        }
        
        private void OnMouseUp()
        {
            if (_data?.MatchBoardController != null)
            {
                OnPieceReleased?.Invoke(this);
            }
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}

