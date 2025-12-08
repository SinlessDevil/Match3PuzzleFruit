using Code.Logic.Level;
using Code.Logic.Level.PM;
using Code.Services.LevelInfo;
using Zenject;

namespace Code.UI.Game
{
    public class RemainingInfoView : LevelTextInfoView 
    {
        private ILevelInfoPM _levelInfoPm;
        private ILevelInfoService _levelInfoService;
        
        [Inject]
        private void Constructor(ILevelInfoService levelInfoService)
        {
            _levelInfoService = levelInfoService;
        }
        
        public override void Initialize(ILevelInfoPM levelInfoPm)
        {
            _levelInfoPm = levelInfoPm;
            
            Subscribe();
            
            UpdateText();
        }

        public override void Dispose()
        {
            Unsubscribe();
        }

        protected override void Subscribe()
        {
            if (_levelInfoService != null)
            {
                _levelInfoService.RemainingChangedEvent += OnRemainingChanged;
            }
        }

        protected override void Unsubscribe()
        {
            if (_levelInfoService != null)
            {
                _levelInfoService.RemainingChangedEvent -= OnRemainingChanged;
            }
        }
        
        private void OnRemainingChanged(string remaining)
        {
            SetText(remaining);
        }
        
        private void UpdateText()
        {
            if (_levelInfoPm != null)
            {
                SetText(_levelInfoPm.GetRemainingText());
            }
        }
    }
}