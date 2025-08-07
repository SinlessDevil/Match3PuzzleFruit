using Code.Services.LevelConductors;
using Code.StaticData.Levels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Match3
{
    public class Hud : MonoBehaviour
    {
        [FormerlySerializedAs("level")] public LevelConductor levelConductor;
        public GameOver gameOver;

        public Text remainingText;
        public Text remainingSubText;
        public Text targetText;
        public Text targetSubtext;
        public Text scoreText;
        public Image[] stars;

        private int _starIndex = 0;

        private void Start ()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].enabled = i == _starIndex;
            }
        }

        public void SetScore(int score)
        {
            scoreText.text = score.ToString();

            int visibleStar = 0;

            if (score >= levelConductor.score1Star && score < levelConductor.score2Star)
            {
                visibleStar = 1;
            }
            else if  (score >= levelConductor.score2Star && score < levelConductor.score3Star)
            {
                visibleStar = 2;
            }
            else if (score >= levelConductor.score3Star)
            {
                visibleStar = 3;
            }

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].enabled = (i == visibleStar);
            }

            _starIndex = visibleStar;
        }

        public void SetTarget(int target) => targetText.text = target.ToString();

        public void SetRemaining(int remaining) => remainingText.text = remaining.ToString();

        public void SetRemaining(string remaining) => remainingText.text = remaining;

        public void SetLevelType(LevelTypeId type)
        {
            switch (type)
            {
                case LevelTypeId.Moves:
                    remainingSubText.text = "moves remaining";
                    targetSubtext.text = "target score";
                    break;
                case LevelTypeId.Obstacle:
                    remainingSubText.text = "moves remaining";
                    targetSubtext.text = "bubbles remaining";
                    break;
                case LevelTypeId.Timer:
                    remainingSubText.text = "time remaining";
                    targetSubtext.text = "target score";
                    break;
            }
        }

        public void OnGameWin(int score)
        {
            gameOver.ShowWin(score, _starIndex);
            if (_starIndex > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0))
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, _starIndex);
            }
        }

        public void OnGameLose() => gameOver.ShowLose();
    }
}
