using Code.Logic.Level;
using Code.Logic.Level.PM;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Game
{
    public abstract class LevelTextInfoView : MonoBehaviour
    {
        [SerializeField] private Text _text;
        
        private string _textConstant = string.Empty;

        public abstract void Initialize(ILevelInfoPM levelInfoPm);
        
        public abstract void Dispose();
        
        protected abstract void Subscribe();
        
        protected abstract void Unsubscribe();
        
        public virtual void SetText(string value)
        {
            if(_textConstant == string.Empty)
            {
                _text.text = value;
                return;
            }
            
            _text.text = value + _textConstant;
        }

        public virtual void SetConstantText(string value)
        {
            _textConstant = value;
        }
    }
}