using Code.Logic.Level.PM;
using Code.Services.LevelInfo;
using UnityEngine;
using Zenject;

namespace Code.UI.Game
{
    public class ScoreInfoView : LevelTextInfoView
    {
        [SerializeField] private GameObject[] _starObjects;
        
        private int _lastStarIndex = 0;
        private ILevelInfoPM _levelInfoPm;
        private ILevelInfoService _levelInfoService;
        
        public int CurrentStarIndex => _lastStarIndex;

        [Inject]
        private void Constructor(ILevelInfoService levelInfoService)
        {
            _levelInfoService = levelInfoService;
        }

        public override void Initialize(ILevelInfoPM levelInfoPm)
        {
            _levelInfoPm = levelInfoPm;
            
            Subscribe();
            
            UpdateScore();
            UpdateStars();
        }

        public override void Dispose()
        {
            Unsubscribe();
        }

        protected override void Subscribe()
        {
            _levelInfoService.ScoreChangedEvent += OnScoreChanged;
        }

        protected override void Unsubscribe()
        {
            _levelInfoService.ScoreChangedEvent -= OnScoreChanged;
        }
        
        private void OnScoreChanged(int score)
        {
            UpdateScore();
            UpdateStars();
        }
        
        private void UpdateScore()
        {
            if (_levelInfoPm != null)
            {
                int score = _levelInfoPm.GetScore();
                SetText(score.ToString());
            }
        }
        
        private void UpdateStars()
        {
            int stars = _levelInfoPm != null ? _levelInfoPm.GetStars() : 0;

            if (_starObjects != null)
            {
                for (int i = 0; i < _starObjects.Length; i++)
                {
                    if (_starObjects[i] != null)
                    {
                        _starObjects[i].SetActive(i < stars);
                    }
                }
            }

            _lastStarIndex = stars;
        }
    }
}