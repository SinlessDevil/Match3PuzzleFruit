using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.Logic.Match3
{
    public class GamePieceView : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
    {
        public event Action<GamePieceView> OnPiecePressed;
        public event Action<GamePieceView> OnPieceEntered;
        public event Action<GamePieceView> OnPieceReleased;
        
        private GamePieceData _data;
        private MovablePieceView _movableComponent;
        private ColorPiece _colorComponent;
        private ClearablePiece _clearableComponent;
        
        public GamePieceData Data => _data;
        public MovablePieceView MovableComponent => _movableComponent;
        public ColorPiece ColorComponent => _colorComponent;
        public ClearablePiece ClearableComponent => _clearableComponent;
        
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
        
        private void Awake()
        {
            _movableComponent = GetComponent<MovablePieceView>();
            _colorComponent = GetComponent<ColorPiece>();
            _clearableComponent = GetComponent<ClearablePiece>();
        }
        
        public void Initialize(GamePieceData data)
        {
            _data = data;
            
            if (_data != null)
            {
                _data.HasMovableComponent = _movableComponent != null;
                _data.HasColorComponent = _colorComponent != null;
                _data.HasClearableComponent = _clearableComponent != null;
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_data?.MatchBoardController != null)
            {
                OnPiecePressed?.Invoke(this);
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_data?.MatchBoardController != null)
            {
                OnPieceEntered?.Invoke(this);
            }
        }
        
        public void OnPointerUp(PointerEventData eventData)
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

