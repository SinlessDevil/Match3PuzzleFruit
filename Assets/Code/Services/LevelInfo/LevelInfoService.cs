using Code.Services.LevelConductors;
using Code.Services.LevelConductors.Locator;
using Code.Services.Levels;
using Code.StaticData.Levels;
using System;

namespace Code.Services.LevelInfo
{
    public class LevelInfoService : ILevelInfoService
    {
        private readonly ILevelServiceLocator _levelServiceLocator;
        private readonly ILevelService _levelService;
        
        private ILevelConductor _currentConductor;
        
        public int CurrentScore { get; private set; }
        public int CurrentStars { get; private set; }
        public string LevelName { get; private set; }
        public string RemainingText { get; private set; }
        public string TargetText { get; private set; }
        public Code.StaticData.Levels.LevelTypeId CurrentLevelTypeId { get; private set; } = Code.StaticData.Levels.LevelTypeId.Moves;
        
        public event Action<int> ScoreChangedEvent;
        public event Action<string> RemainingChangedEvent;
        public event Action<string> TargetChangedEvent;
        
        public LevelInfoService(ILevelServiceLocator levelServiceLocator, ILevelService levelService)
        {
            _levelServiceLocator = levelServiceLocator;
            _levelService = levelService;
        }
        
        public void Initialize()
        {
            if (_currentConductor != null)
            {
                Unsubscribe();
            }
            
            _currentConductor = _levelServiceLocator.GetForCurrentLevel();
            
            LevelStaticData levelData = _levelService.GetCurrentLevelStaticData();
            if (levelData != null)
            {
                LevelName = !string.IsNullOrEmpty(levelData.LevelName) ? levelData.LevelName : $"Level {levelData.LevelId}";
                CurrentLevelTypeId = levelData.LevelTypeId;
            }
            
            Subscribe();
            
            // Получаем начальные значения после подписки
            InitializeInitialValues();
        }
        
        private void Subscribe()
        {
            if (_currentConductor != null)
            {
                _currentConductor.ChangedCurrentScoreEvent += OnScoreChanged;
                _currentConductor.ChangedRemainingEvent += OnRemainingChanged;
                _currentConductor.ChangedTargetEvent += OnTargetChanged;
            }
        }
        
        private void InitializeInitialValues()
        {
            // Начальный счет всегда 0
            CurrentScore = 0;
            UpdateStars();
            ScoreChangedEvent?.Invoke(0);
            
            // RemainingText и TargetText будут установлены через события от кондуктора
            // (они вызываются в конструкторе кондуктора)
        }
        
        private void Unsubscribe()
        {
            if (_currentConductor != null)
            {
                _currentConductor.ChangedCurrentScoreEvent -= OnScoreChanged;
                _currentConductor.ChangedRemainingEvent -= OnRemainingChanged;
                _currentConductor.ChangedTargetEvent -= OnTargetChanged;
            }
        }
        
        private void OnScoreChanged(int score)
        {
            CurrentScore = score;
            UpdateStars();
            ScoreChangedEvent?.Invoke(score);
        }
        
        private void OnRemainingChanged(string remaining)
        {
            RemainingText = remaining;
            RemainingChangedEvent?.Invoke(remaining);
        }
        
        private void OnTargetChanged(string target)
        {
            TargetText = target;
            TargetChangedEvent?.Invoke(target);
        }
        
        private void UpdateStars()
        {
            LevelStaticData levelData = _levelService.GetCurrentLevelStaticData();
            
            if (levelData?.LevelTypeConfigs?.Scores == null || levelData.LevelTypeConfigs.Scores.Count < 3)
            {
                CurrentStars = 0;
                return;
            }
            
            int[] thresholds = new[]
            {
                levelData.LevelTypeConfigs.Scores[0],
                levelData.LevelTypeConfigs.Scores[1],
                levelData.LevelTypeConfigs.Scores[2]
            };
            
            if (CurrentScore >= thresholds[2])
                CurrentStars = 3;
            else if (CurrentScore >= thresholds[1])
                CurrentStars = 2;
            else if (CurrentScore >= thresholds[0])
                CurrentStars = 1;
            else
                CurrentStars = 0;
        }
        
        public void Cleanup()
        {
            Unsubscribe();
            CurrentScore = 0;
            CurrentStars = 0;
            LevelName = string.Empty;
            RemainingText = string.Empty;
            TargetText = string.Empty;
            CurrentLevelTypeId = Code.StaticData.Levels.LevelTypeId.Moves;
            _currentConductor = null;
        }
    }
}

