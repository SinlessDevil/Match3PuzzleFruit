using System;
using Code.Services.Levels;
using Code.StaticData.Levels.LevelTypeConfigs;
using Match3;
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

        public virtual void OnPieceCleared(GamePiece piece)
        {
            _currentScore += piece.score;
            InvokeChangedCurrentScoreEvent();
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
            return;
            
           // gameGrid.GameOver();
            _didWin = true;
           // StartCoroutine(WaitForGridFill());
        }

        protected virtual void GameLose()
        {        
            Debug.Log("Game Lose");
            return;
            
           // gameGrid.GameOver();
           // StartCoroutine(WaitForGridFill());
        }

        // private IEnumerator WaitForGridFill()
        // {
        //     while (gameGrid.IsFilling)
        //     {
        //         yield return null;
        //     }
        //
        //     if (_didWin)
        //     {
        //         hud.OnGameWin(_currentScore);
        //     }
        //     else
        //     {
        //         hud.OnGameLose();
        //     }
        // }
    }
}
