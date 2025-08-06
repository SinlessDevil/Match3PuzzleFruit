using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Game
{
    public abstract class TextView : MonoBehaviour
    {
        [SerializeField] private Text _text;
        
        private string _textConstant = string.Empty;

        public abstract void Subscribe();
        
        public abstract void Unsubscribe();
        
        public virtual void SetText(string value)
        {
            _text.text = value + _textConstant;
        }

        public virtual void SetConstantText(string value)
        {
            _textConstant = value;
        }
    }
}