using Code.StaticData.Levels;

namespace Code.Services.LevelInfo
{
    public interface ILevelInfoService
    {
        int CurrentScore { get; }
        int CurrentStars { get; }
        string LevelName { get; }
        string RemainingText { get; }
        string TargetText { get; }
        Code.StaticData.Levels.LevelTypeId CurrentLevelTypeId { get; }
        
        event System.Action<int> ScoreChangedEvent;
        event System.Action<string> RemainingChangedEvent;
        event System.Action<string> TargetChangedEvent;
        
        void Initialize();
        void Cleanup();
    }
}

