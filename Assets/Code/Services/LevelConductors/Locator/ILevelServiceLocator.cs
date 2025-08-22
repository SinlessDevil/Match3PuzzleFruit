using Code.StaticData.Levels;

namespace Code.Services.LevelConductors.Locator
{
    public interface ILevelServiceLocator
    {
        public ILevelConductor Get(LevelTypeId levelTypeId);
        public ILevelConductor GetForCurrentLevel();
        public void Clear();
    }
}