using Code.Logic.Level.PM;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI.Game
{
    public class ScoreInfoView : LevelTextInfoView
    {
        [SerializeField] private Image[] stars;
        
        private int _lastStarIndex = 0;
        private ILevelInfoPM _levelInfoPm;
        
        public int CurrentStarIndex => _lastStarIndex;

        public override void Initialize(ILevelInfoPM levelInfoPm)
        {
            _levelInfoPm = levelInfoPm;
            
            Subscribe();
        }

        public override void Dispose()
        {
            Unsubscribe();
        }

        protected override void Subscribe()
        {
            
        }

        protected override void Unsubscribe()
        {
            
        }
        
        public void SetScore(int score, int[] thresholds)
        {
            int starIndex = CalculateStars(score, thresholds);

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].enabled = (i == starIndex);
            }

            _lastStarIndex = starIndex;
        }

        private int CalculateStars(int score, int[] thresholds)
        {
            if (thresholds == null || thresholds.Length != 3)
                return 0;

            if (score >= thresholds[2]) return 3;
            if (score >= thresholds[1]) return 2;
            if (score >= thresholds[0]) return 1;

            return 0;
        }
    }
}