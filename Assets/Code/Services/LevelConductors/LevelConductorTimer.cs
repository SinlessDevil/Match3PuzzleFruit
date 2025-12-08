using System.Threading;
using Code.Services.Levels;
using Code.StaticData.Levels.LevelTypeConfigs;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Services.LevelConductors
{
    public class LevelConductorTimer : LevelConductor
    {
        private float _timer;
        private CancellationTokenSource _updateCancellationTokenSource;
        private bool _isGameFinished;

        public LevelConductorTimer(ILevelService levelService) : base(levelService)
        {
            InitTimer();
        }

        public override void OnMove() { }
        
        public override void Dispose()
        {
            base.Dispose();
            _timer = 0f;
            _isGameFinished = false;
            _updateCancellationTokenSource?.Cancel();
            _updateCancellationTokenSource?.Dispose();
            _updateCancellationTokenSource = null;
        }
        
        public void InitTimer()
        {
            _timer = 0f;
            _isGameFinished = false;
            _updateCancellationTokenSource?.Cancel();
            _updateCancellationTokenSource?.Dispose();
            _updateCancellationTokenSource = new CancellationTokenSource();
            
            int maxTime = TimeInSeconds();
            InvokeChangedRemainingEvent($"{FormatTime(maxTime)} из {FormatTime(maxTime)}");
            InvokeChangedTargetEvent(TargetScore().ToString());
            
            UpdateTimerAsync(_updateCancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid UpdateTimerAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && !_isGameFinished)
            {
                await UniTask.Yield(cancellationToken);
                
                _timer += Time.deltaTime;
                
                float remainingTime = TimeInSeconds() - _timer;
                int maxTime = TimeInSeconds();
                
                if (remainingTime <= 0)
                {
                    remainingTime = 0;
                    _isGameFinished = true;
                    
                    if (_currentScore >= TargetScore())
                        GameWin();
                    else
                        GameLose();
                }
                
                InvokeChangedRemainingEvent($"{FormatTime((int)remainingTime)} из {FormatTime(maxTime)}");
            }
        }
        
        private string FormatTime(int totalSeconds)
        {
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            return $"{minutes}:{seconds:00}";
        }
        
        private int TimeInSeconds()
        {
            if (LevelTypeConfigs is LevelTypeConfigTimer levelTypeConfigTimer)
                return levelTypeConfigTimer.TimeInSeconds;

            throw new System.Exception("Level type config is not of type LevelTypeConfigTimer");
        }
        
        private int TargetScore()
        {
            if (LevelTypeConfigs is LevelTypeConfigTimer levelTypeConfigTimer)
                return levelTypeConfigTimer.TargetScore;

            throw new System.Exception("Level type config is not of type LevelTypeConfigTimer");
        }
    }
}
