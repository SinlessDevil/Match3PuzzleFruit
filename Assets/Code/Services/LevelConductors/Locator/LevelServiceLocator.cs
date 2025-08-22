using System;
using Code.Services.Levels;
using Code.StaticData.Levels;
using Zenject;

namespace Code.Services.LevelConductors.Locator
{
    public class LevelServiceLocator : ILevelServiceLocator
    {
        private readonly DiContainer _container;
        private readonly ILevelService _levelService;

        private ILevelConductor _cachedConductor;

        public LevelServiceLocator(DiContainer container, ILevelService levelService)
        {
            _container = container;
            _levelService = levelService;
        }

        public ILevelConductor Get(LevelTypeId levelTypeId)
        {
            return levelTypeId switch
            {
                LevelTypeId.Timer => _container.Instantiate<LevelConductorTimer>(),
                LevelTypeId.Obstacle => _container.Instantiate<LevelConductorObstacles>(),
                LevelTypeId.Moves => _container.Instantiate<LevelConductorMoves>(),
                _ => throw new ArgumentOutOfRangeException(nameof(levelTypeId), levelTypeId, null)
            };
        }

        public ILevelConductor GetForCurrentLevel()
        {
            if (_cachedConductor != null)
                return _cachedConductor;

            var currentLevelData = _levelService.GetCurrentLevelStaticData();
            _cachedConductor = Get(currentLevelData.LevelTypeId);
            return _cachedConductor;
        }

        public void Clear()
        {
            _cachedConductor.Dispose();
            
            if (_cachedConductor is IDisposable disposable)
                disposable.Dispose();

            _cachedConductor = null;
        }
    }
}