using Code.Logic.Level.PM;

namespace Code.UI.Game
{
    public class TargetInfoView : LevelTextInfoView 
    {
        private ILevelInfoPM _levelInfoPm;
        
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
    }
}