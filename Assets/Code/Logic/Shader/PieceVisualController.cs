using UnityEngine;

namespace Code.Logic.Shader
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PieceVisualController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteOutlineGlowView _spriteOutlineGlowView;
        [Header("Colors for different states")]
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _highlightColor = Color.yellow;
        [SerializeField] private Color _selectedColor = Color.cyan;
        [SerializeField] private Color _matchColor = Color.green;
        [SerializeField] private Color _specialColor = Color.magenta;
        [Header("Effects settings")]
        [SerializeField] private float _highlightIntensity = 1.5f;
        [SerializeField] private float _selectedIntensity = 2f;
        [SerializeField] private float _matchIntensity = 2.5f;
        [SerializeField] private float _pulseSpeed = 3f;
        
        private bool _isHighlighted = false;
        private bool _isSelected = false;
        private bool _isMatched = false;
        
        private void Start()
        {
            SetNormalState();
        }
        
        public void SetNormalState()
        {
            _isHighlighted = false;
            _isSelected = false;
            _isMatched = false;
            
            if (_spriteOutlineGlowView != null)
            {
                _spriteOutlineGlowView.SetHighlightMode(false);
                _spriteOutlineGlowView.SetSelectedMode(false);
            }
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _normalColor;
            }
        }
        
        public void SetHighlightState(bool enabled)
        {
            _isHighlighted = enabled;
            
            if (_spriteOutlineGlowView != null)
            {
                _spriteOutlineGlowView.SetHighlightMode(enabled);
            }
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = enabled ? _highlightColor : _normalColor;
            }
        }
        
        public void SetSelectedState(bool enabled)
        {
            _isSelected = enabled;
            
            if (_spriteOutlineGlowView != null)
            {
                _spriteOutlineGlowView.SetSelectedMode(enabled);
            }
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = enabled ? _selectedColor : _normalColor;
            }
        }
        
        public void SetMatchState(bool enabled)
        {
            _isMatched = enabled;
            
            if (_spriteOutlineGlowView != null)
            {
                if (enabled)
                {
                    _spriteOutlineGlowView.SetOutlineEnabled(true);
                    _spriteOutlineGlowView.SetGlowEnabled(true);
                    _spriteOutlineGlowView.SetOutlineColor(_matchColor);
                    _spriteOutlineGlowView.SetGlowColor(_matchColor);
                    _spriteOutlineGlowView.SetOutlineWidth(4f);
                    _spriteOutlineGlowView.SetGlowIntensity(_matchIntensity);
                    _spriteOutlineGlowView.EnableOutlinePulse();
                    _spriteOutlineGlowView.EnableGlowPulse();
                    _spriteOutlineGlowView.SetPulseSpeed(_pulseSpeed);
                }
                else
                {
                    _spriteOutlineGlowView.SetOutlineEnabled(false);
                    _spriteOutlineGlowView.SetGlowEnabled(false);
                    _spriteOutlineGlowView.DisableOutlinePulse();
                    _spriteOutlineGlowView.DisableGlowPulse();
                }
            }
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = enabled ? _matchColor : _normalColor;
            }
        }
        
        public void SetSpecialState(bool enabled)
        {
            if (_spriteOutlineGlowView != null)
            {
                if (enabled)
                {
                    _spriteOutlineGlowView.SetOutlineEnabled(true);
                    _spriteOutlineGlowView.SetGlowEnabled(true);
                    _spriteOutlineGlowView.SetOutlineColor(_specialColor);
                    _spriteOutlineGlowView.SetGlowColor(_specialColor);
                    _spriteOutlineGlowView.SetOutlineWidth(3f);
                    _spriteOutlineGlowView.SetGlowIntensity(2f);
                    _spriteOutlineGlowView.EnableGlowPulse();
                    _spriteOutlineGlowView.SetPulseSpeed(_pulseSpeed * 0.5f);
                }
                else
                {
                    _spriteOutlineGlowView.SetOutlineEnabled(false);
                    _spriteOutlineGlowView.SetGlowEnabled(false);
                    _spriteOutlineGlowView.DisableGlowPulse();
                }
            }
            
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = enabled ? _specialColor : _normalColor;
            }
        }
        
        public void SetPieceColor(Color color)
        {
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = color;
            }
        }
        
        public void SetPulseEnabled(bool enabled)
        {
            if (_spriteOutlineGlowView != null)
            {
                _spriteOutlineGlowView.SetPulseEnabled(enabled);
            }
        }
        
        public void SetPulseSpeed(float speed)
        {
            if (_spriteOutlineGlowView != null)
            {
                _spriteOutlineGlowView.SetPulseSpeed(speed);
            }
        }
        
        public void OnPieceHover()
        {
            if (!_isSelected && !_isMatched)
            {
                SetHighlightState(true);
            }
        }
        
        public void OnPieceExit()
        {
            if (!_isSelected && !_isMatched)
            {
                SetHighlightState(false);
            }
        }
        
        public void OnPiecePress()
        {
            SetSelectedState(true);
        }
        
        public void OnPieceRelease()
        {
            SetSelectedState(false);
        }
        
        public void OnPieceMatch()
        {
            SetMatchState(true);
        }
        
        public void OnPieceClear()
        {
            SetMatchState(false);
            SetNormalState();
        }
        
        public bool IsHighlighted => _isHighlighted;
        
        public bool IsSelected => _isSelected;
        
        public bool IsMatched => _isMatched;
        
        public Color GetCurrentColor()
        {
            return _spriteRenderer != null ? _spriteRenderer.color : _normalColor;
        }
        
        private void OnValidate()
        {
            if (_spriteRenderer != null && !Application.isPlaying)
            {
                _spriteRenderer.color = _normalColor;
            }
        }
        
        private void OnDestroy()
        {
            SetNormalState();
        }
    }
}
