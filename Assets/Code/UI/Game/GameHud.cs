using System.Collections.Generic;
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
        [SerializeField] private RemainingView _remainingView;
        [SerializeField] private TargetView _targetView;
        [SerializeField] private ScoreView _scoreView;
        
        private IStaticDataService _staticDataService; 
        
        [Inject]
        public void Constructor(IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
        }
        
        public InputZona InputZona => _inputZona;
        
        public void Initialize()
        {
            
            InitDebugObjects();
            TrySetUpEventSystem();
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