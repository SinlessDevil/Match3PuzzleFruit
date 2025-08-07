using System.Collections.Generic;
using Code.Logic.Level;
using Code.Logic.Level.PM;
using Code.Services.StaticData;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Code.UI.Game
{
    public class GameHud : MonoBehaviour
    {
        [Space(10)] [Header("Other")]
        [SerializeField] private InputZona _inputZona;
        [SerializeField] private List<GameObject> _debugObjects;
        [Space(10)] [Header("Views")]
        [SerializeField] private RemainingInfoView remainingInfoView;
        [SerializeField] private TargetInfoView targetInfoView;
        [SerializeField] private ScoreInfoView scoreInfoView;
        
        private IStaticDataService _staticDataService; 
        
        [Inject]
        public void Constructor(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }
        
        public InputZona InputZona => _inputZona;
        
        public void Initialize(ILevelInfoPM levelInfoPm)
        {
            remainingInfoView.Initialize(levelInfoPm);
            targetInfoView.Initialize(levelInfoPm);
            scoreInfoView.Initialize(levelInfoPm);
            
            InitDebugObjects();
            TrySetUpEventSystem();
        }
        
        public void Dispose()
        {
            remainingInfoView.Dispose();
            targetInfoView.Dispose();
            scoreInfoView.Dispose();
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