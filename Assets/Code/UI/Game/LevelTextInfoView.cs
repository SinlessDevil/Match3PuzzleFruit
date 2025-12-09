using Code.Logic.Level.PM;
using UnityEngine;
using TMPro;

namespace Code.UI.Game
{
    public abstract class LevelTextInfoView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        
        private string _textConstant = string.Empty;

        public abstract void Initialize(ILevelInfoPM levelInfoPm);
        
        public abstract void Dispose();
        
        protected abstract void Subscribe();
        
        protected abstract void Unsubscribe();

        protected void SetText(string value)
        {
            if(_textConstant == string.Empty)
            {
                _text.text = value;
                return;
            }
            
            _text.text = value + _textConstant;
        }

        public void SetConstantText(string value)
        {
            _textConstant = value;
        }
    }
}