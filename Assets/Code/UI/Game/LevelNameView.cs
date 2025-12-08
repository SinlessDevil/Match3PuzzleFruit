using Code.Logic.Level.PM;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Game
{
    public class LevelNameView : MonoBehaviour
    {
        [SerializeField] private Text _text;
        
        private ILevelInfoPM _levelInfoPm;
        
        public void Initialize(ILevelInfoPM levelInfoPm)
        {
            _levelInfoPm = levelInfoPm;
            
            UpdateText();
        }
        
        private void UpdateText()
        {
            if (_text != null && _levelInfoPm != null)
            {
                _text.text = $"Level {_levelInfoPm.GetLevelNumber()}";
            }
        }
    }
}

