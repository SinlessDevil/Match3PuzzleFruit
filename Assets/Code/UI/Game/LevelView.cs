using Code.Logic.Level.PM;
using UnityEngine;
using TMPro;

namespace Code.UI.Game
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        
        private ILevelInfoPM _levelInfoPm;
        
        public void Initialize(ILevelInfoPM levelInfoPm)
        {
            _levelInfoPm = levelInfoPm;
            UpdateText();
        }
        
        private void UpdateText()
        {
            int levelNumber = _levelInfoPm.GetLevelNumber();
            _text.text = $"Level {levelNumber}";
        }
    }
}
