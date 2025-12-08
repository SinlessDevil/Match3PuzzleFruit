using UnityEngine;

namespace Code.UI.Menu.Windows
{
    public abstract class BaseWindow : MonoBehaviour
    {
        [SerializeField] private TypeWindow _typeWindow;
        [SerializeField] private RectTransform _rectTransform;

        private void OnValidate()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
        }

        public TypeWindow TypeWindow => _typeWindow;
        
        public RectTransform RectTransform => _rectTransform;
        
        public abstract void Initialize();
    }
}