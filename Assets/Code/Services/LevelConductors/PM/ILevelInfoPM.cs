using Code.StaticData.Levels;

namespace Code.Logic.Level.PM
{
    public interface ILevelInfoPM
    {
        int GetScore();
        string GetTargetText();
        string GetRemainingText();
        string GetLevelName();
        int GetLevelNumber();
        int GetStars();
        LevelTypeId GetCurrentLevelTypeId();
        string GetConstantLevelText(TextTypeId textTypeId, LevelTypeId levelTypeId);
    }
}