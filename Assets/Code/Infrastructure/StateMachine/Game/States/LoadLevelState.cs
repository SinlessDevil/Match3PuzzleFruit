using Code.Logic.Level.PM;
using Code.Services.Factories.UIFactory;
using Code.Services.LevelConductors.Locator;
using Code.Services.Levels;
using Code.Services.Providers.Widgets;
using Code.Services.StaticData;
using Code.UI.Game;

namespace Code.Infrastructure.StateMachine.Game.States
{
    public class LoadLevelState : IPayloadedState<string>, IGameState
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly ILoadingCurtain _loadingCurtain;
        private readonly IUIFactory _uiFactory;
        private readonly IStateMachine<IGameState> _gameStateMachine;
        private readonly IWidgetProvider _widgetProvider;
        private readonly ILevelService _levelService;
        private readonly IStaticDataService _staticDataService;
        private readonly ILevelServiceLocator _levelServiceLocator;

        public LoadLevelState(
            IStateMachine<IGameState> gameStateMachine, 
            ISceneLoader sceneLoader,
            ILoadingCurtain loadingCurtain, 
            IUIFactory uiFactory,
            IWidgetProvider widgetProvider,
            ILevelService levelService,
            IStaticDataService staticDataService,
            ILevelServiceLocator levelServiceLocator)
        {
            _gameStateMachine = gameStateMachine;
            _sceneLoader = sceneLoader;
            _loadingCurtain = loadingCurtain;
            _uiFactory = uiFactory;
            _widgetProvider = widgetProvider;
            _levelService = levelService;
            _staticDataService = staticDataService;
            _levelServiceLocator = levelServiceLocator;
        }

        public void Enter(string payload)
        {
            _loadingCurtain.Show();
            _sceneLoader.Load(payload, OnLevelLoad);
        }

        public void Exit()
        {
            _loadingCurtain.Hide();
        }

        protected virtual void OnLevelLoad()
        {
            InitGameWorld();

            _gameStateMachine.Enter<GameLoopState>();
        }

        private void InitGameWorld()
        {
            _uiFactory.CreateUiRoot();

            InitLevelServiceConductor();
            
            ILevelInfoPM levelInfoPm = CreateLevelInfoPM();
            
            InitHud(levelInfoPm);
            
            InitProviders();
        }

        private void InitLevelServiceConductor()
        {
            _levelServiceLocator.GetForCurrentLevel();
        }

        private void InitProviders()
        {
            _widgetProvider.CreatePoolWidgets();
        }
        
        private void InitHud(ILevelInfoPM levelInfoPm)
        {
            GameHud gameHud = _uiFactory.CreateGameHud();
            gameHud.Initialize(levelInfoPm);
        }

        private ILevelInfoPM CreateLevelInfoPM()
        {
            return new LevelInfoPm(_levelService, _staticDataService);
        }
    }
}