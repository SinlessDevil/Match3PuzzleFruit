using System;
using Code.Logic.Match3;
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

        protected LevelConductor(ILevelService levelService)
        {
            _levelService = levelService;
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
        
        protected void InvokeChangedCurrentScoreEvent() => ChangedCurrentScoreEvent?.Invoke(_currentScore);

        protected void InvokeChangedRemainingEvent(string remaining) => ChangedRemainingEvent?.Invoke(remaining);

        protected void InvokeChangedTargetEvent(string target) => ChangedTargetEvent?.Invoke(target);

        protected virtual void GameWin()
        {
            Debug.Log("Game Win");
        }

        protected virtual void GameLose()
        {        
            Debug.Log("Game Lose");
        }
    }
}
