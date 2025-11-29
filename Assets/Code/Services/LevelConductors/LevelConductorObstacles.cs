using Code.Logic.Match3;
using Code.Services.Levels;
using Code.StaticData.Levels.LevelTypeConfigs;

namespace Code.Services.LevelConductors
{
    public class LevelConductorObstacles : LevelConductor
    {
        private const int ScorePerPieceCleared = 1000;
    
        private int _movesUsed = 0;
        private int _numObstaclesLeft = 0;

        public LevelConductorObstacles(ILevelService levelService) : base(levelService) { }

        public void InitNumObstaclesLeft()
        {
            // for (int i = 0; i < obstacleTypes.Length; i++)
            // {
            //     _numObstaclesLeft += matchBoardController.GetPiecesOfType(obstacleTypes[i]).Count;
            // }

            InvokeChangedTargetEvent(_numObstaclesLeft.ToString());
        }
        
        public override void OnMove()
        {
            _movesUsed++;

            InvokeChangedRemainingEvent((NumMoves() - _movesUsed).ToString());
            
            if (NumMoves() - _movesUsed == 0 && _numObstaclesLeft > 0)
                GameLose();
        }

        public override void Dispose()
        {
            base.Dispose();
            _movesUsed = 0;
            _numObstaclesLeft = 0;
        }

        public override void OnPieceCleared(GamePiece piece)
        {
            base.OnPieceCleared(piece);

            foreach (var obstacle in PieceTypes())
            {
                if (obstacle != piece.Type) 
                    continue;
            
                _numObstaclesLeft--;
                InvokeChangedTargetEvent(_numObstaclesLeft.ToString());
                
                if (_numObstaclesLeft != 0) 
                    continue;
            
                _currentScore += ScorePerPieceCleared * (NumMoves() - _movesUsed);
                InvokeChangedCurrentScoreEvent();
                
                GameWin();
            }
        }
        
        private int NumMoves()
        {
            if (LevelTypeConfigs is LevelTypeConfigObstacles levelTypeConfigMoves)
                return levelTypeConfigMoves.NumMoves;

            throw new System.Exception("Level type config is not of type LevelTypeConfigMoves");
        }
        
        private PieceType[] PieceTypes()
        {
            if (LevelTypeConfigs is LevelTypeConfigObstacles levelTypeConfigMoves)
                return levelTypeConfigMoves.ObstacleTypes;

            throw new System.Exception("Level type config is not of type LevelTypeConfigMoves");
        }
    }
}
