using System.Collections;
using Code.StaticData.Levels;
using Match3;
using UnityEngine;

namespace Code.Services.LevelConductors
{
    public class LevelConductor : MonoBehaviour
    {
        public GameGrid gameGrid;
        public Hud hud;

        public int score1Star;
        public int score2Star;
        public int score3Star;    

        protected LevelTypeId type;

        protected int currentScore;

        private bool _didWin;

        private void Start()
        {
            hud.SetScore(currentScore);
        }

        public LevelTypeId Type => type;

        public virtual void OnMove() { }

        public virtual void OnPieceCleared(GamePiece piece)
        {
            currentScore += piece.score;
            hud.SetScore(currentScore);
        }
        
        protected virtual void GameWin()
        {
            Debug.Log("Game Win");
            return;
            
            gameGrid.GameOver();
            _didWin = true;
            StartCoroutine(WaitForGridFill());
        }

        protected virtual void GameLose()
        {        
            Debug.Log("Game Lose");
            return;
            
            gameGrid.GameOver();
            _didWin = false;
            StartCoroutine(WaitForGridFill());
        }

        private IEnumerator WaitForGridFill()
        {
            while (gameGrid.IsFilling)
            {
                yield return null;
            }

            if (_didWin)
            {
                hud.OnGameWin(currentScore);
            }
            else
            {
                hud.OnGameLose();
            }
        }
    }
}
