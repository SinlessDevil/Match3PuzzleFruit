using Code.Services.Levels;
using Code.StaticData.Levels.LevelTypeConfigs;
using UnityEngine;

namespace Code.Services.LevelConductors
{
    public class LevelConductorTimer : LevelConductor
    {
        private float _timer;

        public LevelConductorTimer(ILevelService levelService) : base(levelService) { }

        public override void OnMove() { }
        
        public override void Dispose()
        {
            base.Dispose();
            _timer = 0f;
        }
        
        public void InitTimer()
        {
            InvokeChangedRemainingEvent($"{TimeInSeconds() / 60}:{TimeInSeconds() % 60:00}");
        }

        public void Update()
        {
            _timer += Time.deltaTime;
            
            InvokeChangedRemainingEvent($"{(int) Mathf.Max((TimeInSeconds() - _timer) / 60, 0)}:" +
                                        $"{(int) Mathf.Max((TimeInSeconds() - _timer) % 60, 0):00}");

            if (!(TimeInSeconds() - _timer <= 0)) 
                return;
            
            if (_currentScore >= TargetScore())
                GameWin();
            else
                GameLose();
        }
        
        private int TimeInSeconds()
        {
            if (LevelTypeConfigs is LevelTypeConfigTimer levelTypeConfigMoves)
                return levelTypeConfigMoves.TimeInSeconds;

            throw new System.Exception("Level type config is not of type LevelTypeConfigMoves");
        }
        
        private int TargetScore()
        {
            if (LevelTypeConfigs is LevelTypeConfigTimer levelTypeConfigMoves)
                return levelTypeConfigMoves.TargetScore;

            throw new System.Exception("Level type config is not of type LevelTypeConfigMoves");
        }
    }
}
