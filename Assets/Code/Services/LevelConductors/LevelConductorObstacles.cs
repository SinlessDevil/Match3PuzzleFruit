using Code.Logic.Controllers;
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

        public void InitNumObstaclesLeft(IMatchBoardController matchBoardController)
        {
            _numObstaclesLeft = 0;
            
            PieceType[] obstacleTypes = PieceTypes();
            
            for (int i = 0; i < obstacleTypes.Length; i++)
            {
                _numObstaclesLeft += matchBoardController.GetPiecesOfType(obstacleTypes[i]).Count;
            }

            InvokeChangedTargetEvent(_numObstaclesLeft.ToString());
            int maxMoves = NumMoves();
            InvokeChangedRemainingEvent($"{maxMoves} из {maxMoves}");
        }
        
        public override void OnMove()
        {
            _movesUsed++;

            int maxMoves = NumMoves();
            int remaining = maxMoves - _movesUsed;
            InvokeChangedRemainingEvent($"{remaining} из {maxMoves}");
            
            if (NumMoves() - _movesUsed == 0 && _numObstaclesLeft > 0)
                GameLose();
        }

        public override void Dispose()
        {
            base.Dispose();
            _movesUsed = 0;
            _numObstaclesLeft = 0;
        }

        public override void OnPieceCleared(GamePieceView pieceView)
        {
            base.OnPieceCleared(pieceView);

            if (pieceView?.Data == null)
                return;

            foreach (PieceType obstacle in PieceTypes())
            {
                if (obstacle != pieceView.Data.Type) 
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
            if (LevelTypeConfigs is LevelTypeConfigObstacles levelTypeConfigObstacles)
                return levelTypeConfigObstacles.NumMoves;

            throw new System.Exception("Level type config is not of type LevelTypeConfigObstacles");
        }
        
        private PieceType[] PieceTypes()
        {
            if (LevelTypeConfigs is LevelTypeConfigObstacles levelTypeConfigObstacles)
                return levelTypeConfigObstacles.ObstacleTypes;

            throw new System.Exception("Level type config is not of type LevelTypeConfigObstacles");
        }
    }
}
