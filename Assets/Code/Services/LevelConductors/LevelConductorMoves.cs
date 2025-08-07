using Code.StaticData.Levels;

namespace Code.Services.LevelConductors
{
    public class LevelConductorMoves : LevelConductor
    {

        public int numMoves;
        public int targetScore;

        private int _movesUsed = 0;

        private void Start()
        {
            type = LevelTypeId.Moves;

            hud.SetLevelType(type);
            hud.SetScore(currentScore);
            hud.SetTarget(targetScore);
            hud.SetRemaining(numMoves);
        }

        public override void OnMove()
        {
            _movesUsed++;

            hud.SetRemaining(numMoves - _movesUsed);

            if (numMoves - _movesUsed != 0) return;
        
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
