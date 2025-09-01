using UnityEngine;

namespace Code.Logic.Shader
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteOutlineGlowView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [Header("Outline settings")]
        [SerializeField] private Color _outlineColor = Color.white;
        [SerializeField] private float _outlineWidth = 1f;
        [SerializeField] private float _outlineAlpha = 1f;
        [SerializeField] private bool _enableOutline = true;
        [Header("Glow settings")]
        [SerializeField] private Color _glowColor = Color.red;
        [SerializeField] private float _glowIntensity = 1f;
        [SerializeField] private float _glowPower = 2f;
        [SerializeField] private float _glowAlpha = 0.5f;
        [SerializeField] private bool _enableGlow = true;
        [Header("Pulsation settings")]
        [SerializeField] private float _pulseSpeed = 2f;
        [SerializeField] private float _pulseIntensity = 0.3f;
        [SerializeField] private bool _enablePulse = false;
        [SerializeField] private bool _outlinePulse = false;
        [SerializeField] private bool _glowPulse = false;
        
        private Material _material;
        private bool _isInitialized = false;
        
        private void Awake()
        {
            InitializeMaterial();
        }
        
        private void InitializeMaterial()
        {
            if (_spriteRenderer == null) 
                return;
            
            _material = new Material(_spriteRenderer.material);
            _spriteRenderer.material = _material;
            
            ApplySettings();
            _isInitialized = true;
        }
        
        private void ApplySettings()
        {
            if (_material == null) 
                return;
            
            _material.SetColor("_OutlineColor", _outlineColor);
            _material.SetFloat("_OutlineWidth", _outlineWidth);
            _material.SetFloat("_OutlineAlpha", _outlineAlpha);
            _material.SetFloat("_EnableOutline", _enableOutline ? 1f : 0f);
            
            _material.SetColor("_GlowColor", _glowColor);
            _material.SetFloat("_GlowIntensity", _glowIntensity);
            _material.SetFloat("_GlowPower", _glowPower);
            _material.SetFloat("_GlowAlpha", _glowAlpha);
            _material.SetFloat("_EnableGlow", _enableGlow ? 1f : 0f);
            
            _material.SetFloat("_PulseSpeed", _pulseSpeed);
            _material.SetFloat("_PulseIntensity", _pulseIntensity);
            _material.SetFloat("_EnablePulse", _enablePulse ? 1f : 0f);
            _material.SetFloat("_OutlinePulse", _outlinePulse ? 1f : 0f);
            _material.SetFloat("_GlowPulse", _glowPulse ? 1f : 0f);
        }
        
        public void SetOutlineEnabled(bool enabled)
        {
            _enableOutline = enabled;
            if (_isInitialized)
                _material.SetFloat("_EnableOutline", enabled ? 1f : 0f);
        }
        
        public void SetOutlineColor(Color color)
        {
            _outlineColor = color;
            if (_isInitialized)
                _material.SetColor("_OutlineColor", color);
        }
        
        public void SetOutlineWidth(float width)
        {
            _outlineWidth = width;
            if (_isInitialized)
                _material.SetFloat("_OutlineWidth", width);
        }
        
        public void SetGlowEnabled(bool enabled)
        {
            _enableGlow = enabled;
            if (_isInitialized)
                _material.SetFloat("_EnableGlow", enabled ? 1f : 0f);
        }
        
        public void SetGlowColor(Color color)
        {
            _glowColor = color;
            if (_isInitialized)
                _material.SetColor("_GlowColor", color);
        }
        
        public void SetGlowIntensity(float intensity)
        {
            _glowIntensity = intensity;
            if (_isInitialized)
                _material.SetFloat("_GlowIntensity", intensity);
        }
        
        public void SetPulseEnabled(bool enabled)
        {
            _enablePulse = enabled;
            if (_isInitialized)
                _material.SetFloat("_EnablePulse", enabled ? 1f : 0f);
        }
        
        public void SetPulseSpeed(float speed)
        {
            _pulseSpeed = speed;
            if (_isInitialized)
                _material.SetFloat("_PulseSpeed", speed);
        }
        
        public void SetPulseIntensity(float intensity)
        {
            _pulseIntensity = intensity;
            if (_isInitialized)
                _material.SetFloat("_PulseIntensity", intensity);
        }
        
        public void EnableOutlinePulse()
        {
            _outlinePulse = true;
            if (_isInitialized)
                _material.SetFloat("_OutlinePulse", 1f);
        }
        
        public void DisableOutlinePulse()
        {
            _outlinePulse = false;
            if (_isInitialized)
                _material.SetFloat("_OutlinePulse", 0f);
        }
        
        public void EnableGlowPulse()
        {
            _glowPulse = true;
            if (_isInitialized)
                _material.SetFloat("_GlowPulse", 1f);
        }
        
        public void DisableGlowPulse()
        {
            _glowPulse = false;
            if (_isInitialized)
                _material.SetFloat("_GlowPulse", 0f);
        }
        
        public void SetHighlightMode(bool enabled)
        {
            if (enabled)
            {
                SetOutlineEnabled(true);
                SetGlowEnabled(true);
                SetOutlineColor(Color.yellow);
                SetGlowColor(Color.yellow);
                SetOutlineWidth(2f);
                SetGlowIntensity(2f);
            }
            else
            {
                SetOutlineEnabled(false);
                SetGlowEnabled(false);
            }
        }
        
        public void SetSelectedMode(bool enabled)
        {
            if (enabled)
            {
                SetOutlineEnabled(true);
                SetGlowEnabled(true);
                SetOutlineColor(Color.cyan);
                SetGlowColor(Color.cyan);
                SetOutlineWidth(3f);
                SetGlowIntensity(3f);
                EnableOutlinePulse();
                EnableGlowPulse();
            }
            else
            {
                SetOutlineEnabled(false);
                SetGlowEnabled(false);
                DisableOutlinePulse();
                DisableGlowPulse();
            }
        }
        
        private void OnValidate()
        {
            if (_isInitialized)
            {
                ApplySettings();
            }
        }
        
        private void OnDestroy()
        {
            if (_material != null)
            {
                DestroyImmediate(_material);
            }
        }
    }
}