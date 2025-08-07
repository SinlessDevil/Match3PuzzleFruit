using Code.StaticData.Levels;
using UnityEngine;

namespace Code.Services.LevelConductors
{
    public class LevelConductorTimer : LevelConductor
    {

        public int timeInSeconds;
        public int targetScore;

        private float _timer;

        private void Start ()
        {
            type = LevelTypeId.Timer;

            hud.SetLevelType(type);
            hud.SetScore(currentScore);
            hud.SetTarget(targetScore);
            hud.SetRemaining($"{timeInSeconds / 60}:{timeInSeconds % 60:00}");
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            
            hud.SetRemaining(
                $"{(int) Mathf.Max((timeInSeconds - _timer) / 60, 0)}:" +
                $"{(int) Mathf.Max((timeInSeconds - _timer) % 60, 0):00}");

            if (timeInSeconds - _timer <= 0)
            {
                if (currentScore >= targetScore)
                {
                    GameWin();
                }
                else
                {
                    GameLose();
                }
            }
        }
	
    }
}
