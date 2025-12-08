using System.Collections.Generic;
using Code.Logic.Level.PM;
using Code.Services.StaticData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Zenject;

namespace Code.UI.Game
{
    public class GameHud : MonoBehaviour
    {
        [Space(10)] [Header("Other")]
        [SerializeField] private InputZona _inputZona;
        [SerializeField] private List<GameObject> _debugObjects;
        [FormerlySerializedAs("_levelNameView")]
        [FormerlySerializedAs("levelNameView")]
        [Space(10)] [Header("Views")]
        [SerializeField] private LevelView levelView;
        [SerializeField] private RemainingInfoView _remainingInfoView;
        [FormerlySerializedAs("_targetInfoView")]
        [SerializeField] private LevelTypeView _levelTypeView;
        [SerializeField] private ScoreInfoView _scoreInfoView;
        
        private IStaticDataService _staticDataService; 
        
        [Inject]
        public void Constructor(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }
        
        public InputZona InputZona => _inputZona;
        
        public void Initialize(ILevelInfoPM levelInfoPm)
        {
            if (levelView != null)
                levelView.Initialize(levelInfoPm);
            
            if (_remainingInfoView != null)
                _remainingInfoView.Initialize(levelInfoPm);
            
            if (_levelTypeView != null)
                _levelTypeView.Initialize(levelInfoPm);
            
            if (_scoreInfoView != null)
                _scoreInfoView.Initialize(levelInfoPm);
            
            InitDebugObjects();
            TrySetUpEventSystem();
        }
        
        public void Dispose()
        {
            if (_remainingInfoView != null)
                _remainingInfoView.Dispose();
            
            if (_scoreInfoView != null)
                _scoreInfoView.Dispose();
        }
        
        private void InitDebugObjects()
        {
            if (!_staticDataService.GameConfig.DebugMode)
                return;
            
            foreach (var debugObject in _debugObjects)
            {
                debugObject.SetActive(true);
            }
        }

        private void TrySetUpEventSystem()
        {
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null) 
                return;
            
            GameObject gameObjectEventSystem = new GameObject("EventSystem");
            gameObjectEventSystem.AddComponent<EventSystem>();
            gameObjectEventSystem.AddComponent<StandaloneInputModule>();
        }
    }
}