using Code.Services.Levels;
using Code.StaticData.Levels.LevelTypeConfigs;

namespace Code.Services.LevelConductors
{
    public class LevelConductorMoves : LevelConductor
    {
        private int _movesUsed = 0;

        public LevelConductorMoves(ILevelService levelService) : base(levelService) { }

        public override void Dispose()
        {
            base.Dispose();
            
            _movesUsed = 0;
        }
        
        public override void OnMove()
        {
            _movesUsed++;
            
            InvokeChangedRemainingEvent((NumMoves() - _movesUsed).ToString());
            
            if (NumMoves() - _movesUsed != 0)
                return;
        
            if (_currentScore >= TargetScore())
                GameWin();
            else
                GameLose();
        }
        
        private int NumMoves()
        {
            if (LevelTypeConfigs is LevelTypeConfigMoves levelTypeConfigMoves)
                return levelTypeConfigMoves.NumMoves;

            throw new System.Exception("Level type config is not of type LevelTypeConfigMoves");
        }

        private int TargetScore()
        {
            if (LevelTypeConfigs is LevelTypeConfigMoves levelTypeConfigMoves)
                return levelTypeConfigMoves.TargetScore;
            
            throw new System.Exception("Level type config is not of type LevelTypeConfigMoves");
        }
    }
}
