using System;
using Code.Logic.Match3;
using Code.Services.Finish;
using Code.Services.Levels;
using Code.StaticData.Levels.LevelTypeConfigs;
using UnityEngine;

namespace Code.Services.LevelConductors
{
    public abstract class LevelConductor : ILevelConductor
    {
        protected int _currentScore;
        private bool _didWin;

        private readonly ILevelService _levelService;
        protected IFinishService _finishService;

        protected LevelConductor(ILevelService levelService)
        {
            _levelService = levelService;
        }
        
        public void SetFinishService(IFinishService finishService)
        {
            _finishService = finishService;
        }
        
        public event Action<int> ChangedCurrentScoreEvent;
        public event Action<string> ChangedRemainingEvent;
        public event Action<string> ChangedTargetEvent;
        
        public abstract void OnMove();

        public virtual void OnPieceCleared(GamePieceView pieceView)
        {
            if (pieceView?.Data != null)
            {
                _currentScore += pieceView.Data.Score;
                InvokeChangedCurrentScoreEvent();
            }
        }

        public virtual void Dispose()
        {
            _currentScore = 0;
            _didWin = false;
        }
        
        protected LevelTypeConfig LevelTypeConfigs => _levelService.GetCurrentLevelStaticData().LevelTypeConfigs;
        
        protected int GetCurrentStars()
        {
            if (LevelTypeConfigs?.Scores == null || LevelTypeConfigs.Scores.Count < 3)
                return 0;
            
            int[] thresholds = new[]
            {
                LevelTypeConfigs.Scores[0],
                LevelTypeConfigs.Scores[1],
                LevelTypeConfigs.Scores[2]
            };
            
            if (_currentScore >= thresholds[2])
                return 3;
            if (_currentScore >= thresholds[1])
                return 2;
            if (_currentScore >= thresholds[0])
                return 1;
            
            return 0;
        }
        
        protected void InvokeChangedCurrentScoreEvent() => ChangedCurrentScoreEvent?.Invoke(_currentScore);

        protected void InvokeChangedRemainingEvent(string remaining) => ChangedRemainingEvent?.Invoke(remaining);

        protected void InvokeChangedTargetEvent(string target) => ChangedTargetEvent?.Invoke(target);

        protected virtual void GameWin()
        {
            if (_didWin)
                return;
            
            _didWin = true;
            
            int stars = GetCurrentStars();
            
            if (stars >= 1 && _finishService != null)
            {
                _finishService.Win();
            }
            else if (_finishService != null)
            {
                _finishService.Lose();
            }
        }

        protected virtual void GameLose()
        {
            if (_didWin)
                return;
            
            _didWin = true;
            
            if (_finishService != null)
            {
                _finishService.Lose();
            }
        }
    }
}
