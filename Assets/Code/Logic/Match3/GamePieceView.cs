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
            }
        }
        
        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}

