using Code.Logic.Controllers;
using Code.Logic.Holders;
using Code.Services.Factories.Pieces;
using Code.Services.Factories.UIFactory;
using Code.Services.Input;
using Code.Services.LevelConductors.Locator;
using Code.Services.Levels;
using Code.Services.LocalProgress;
using Code.Services.Providers.Widgets;
using Code.Services.Timer;
using UnityEngine;

namespace Code.Infrastructure.StateMachine.Game.States
{
    public class GameLoopState : IState, IGameState, IUpdatable
    {
        private readonly IStateMachine<IGameState> _gameStateMachine;
        private readonly IInputService _inputService;
        private readonly IWidgetProvider _widgetProvider;
        private readonly ILevelService _levelService;
        private readonly ILevelLocalProgressService _levelLocalProgressService;
        private readonly ITimeService _timeService;
        private readonly IUIFactory _uiFactory;
        private readonly ILevelServiceLocator _levelServiceLocator;
        private readonly IPieceFactory _pieceFactory;
        private readonly ICameraAdapterService _cameraAdapterService;
        
        private IMatchBoardController _matchBoardController;
        
        public GameLoopState(
            IStateMachine<IGameState> gameStateMachine, 
            IInputService inputService,
            IWidgetProvider widgetProvider,
            ILevelService levelService,
            ILevelLocalProgressService levelLocalProgressService,
            ITimeService timeService,
            IUIFactory uiFactory,
            ILevelServiceLocator levelServiceLocator,
            IPieceFactory pieceFactory,
            ICameraAdapterService cameraAdapterService)
        {
            _gameStateMachine = gameStateMachine;
            _inputService = inputService;
            _widgetProvider = widgetProvider;
            _levelService = levelService;
            _levelLocalProgressService = levelLocalProgressService;
            _timeService = timeService;
            _uiFactory = uiFactory;
            _levelServiceLocator = levelServiceLocator;
            _pieceFactory = pieceFactory;
            _cameraAdapterService = cameraAdapterService;
        }
        
        public void Enter()
        {
            InitCameraAdapter();

            InitMatchBoardController();
        }

        private void InitCameraAdapter()
        {
            _cameraAdapterService.Initialize(GetMapHolder());
        }

        private void InitMatchBoardController()
        {
            _matchBoardController = new MatchBoardController(
                _levelServiceLocator,
                _levelService,
                _pieceFactory,
                _cameraAdapterService);
            
            _matchBoardController.SetRootTransform(GetMapHolder().transform);
            
            _matchBoardController.StartLevel();
        }

        public void Update()
        {
            
        }

        public void Exit()
        {
            _matchBoardController.Dispose();
            _cameraAdapterService.Dispose();
            
            if(_uiFactory.GameHud != null)
                _uiFactory.GameHud.Dispose();
            
            _levelServiceLocator.Clear();
            
            _inputService.Cleanup();
            _widgetProvider.CleanupPool();
            _levelService.Cleanup();
            _levelLocalProgressService.Cleanup();
            
            _timeService.ResetTimer();
        }

        private MapHolder GetMapHolder() => Object.FindAnyObjectByType<MapHolder>();
    }
}