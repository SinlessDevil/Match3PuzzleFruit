using Code.Logic.Level.PM;
using Code.Services.LevelInfo;
using Code.StaticData.Levels;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Code.UI.Game
{
    public class LevelTypeView : MonoBehaviour
    {
        [SerializeField] private Text _text;
        
        private ILevelInfoPM _levelInfoPm;
        private ILevelInfoService _levelInfoService;
        
        [Inject]
        private void Constructor(ILevelInfoService levelInfoService)
        {
            _levelInfoService = levelInfoService;
        }
        
        public void Initialize(ILevelInfoPM levelInfoPm)
        {
            _levelInfoPm = levelInfoPm;
            
            UpdateText();
        }
        
        private void UpdateText()
        {
            if (_text != null && _levelInfoPm != null)
            {
                LevelTypeId levelType = _levelInfoPm.GetCurrentLevelTypeId();
                string levelTypeName = GetLevelTypeName(levelType);
                _text.text = levelTypeName;
            }
        }
        
        private string GetLevelTypeName(LevelTypeId levelTypeId)
        {
            return levelTypeId switch
            {
                LevelTypeId.Moves => "Moves",
                LevelTypeId.Timer => "Timer",
                LevelTypeId.Obstacle => "Obstacle",
                _ => "Unknown"
            };
        }
    }
}


