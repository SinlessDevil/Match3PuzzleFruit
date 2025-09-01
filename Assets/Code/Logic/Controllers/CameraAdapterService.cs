using Code.Logic.Holders;
using Code.Services.Levels;
using Code.StaticData.Levels.BoardConfigs;
using UnityEngine;

namespace Code.Logic.Controllers
{
    public class CameraAdapterService : ICameraAdapterService
    {
        private float _padding = 1f;
        private bool _autoCenterCamera = true;
        
        private Camera _camera;
        private MapHolder _mapHolder;
     
        private readonly ILevelService _levelService;
        
        public CameraAdapterService(ILevelService levelService)
        {
            _levelService = levelService;
        }
        
        public void Initialize(MapHolder mapHolder)
        {
            _camera = Camera.main;
            _mapHolder = mapHolder;
            
            AdaptCameraToBoard();
        }

        public void Dispose()
        {
            _camera = null;
            _mapHolder = null;
        }
        
        public void AdaptCameraToBoard()
        {
            if (_camera == null || BoardConfig == null)
                return;
            
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float aspectRatio = screenWidth / screenHeight;
            
            float boardWidth = BoardConfig.XDim;
            float boardHeight = BoardConfig.YDim;
            
            float targetWidth = boardWidth + _padding * 2;
            float targetHeight = boardHeight + _padding * 2;
            
            float orthographicSize;
            
            if (aspectRatio > 1f)
            {
                orthographicSize = targetHeight / 2f;
            }
            else 
            {
                float targetWidthInUnits = targetWidth / aspectRatio;
                orthographicSize = targetWidthInUnits / 2f;
            }
            
            _camera.orthographicSize = orthographicSize;
            
            if (_autoCenterCamera && _mapHolder.transform != null)
            {
                CenterCameraOnBoard();
            }
        }
        
        public void CenterCameraOnBoard()
        {
            if (_mapHolder == null || _camera == null)
                return;
            
            float centerX = _mapHolder.transform.position.x + _mapHolder.transform.position.x;
            float centerY = _mapHolder.transform.position.y + _mapHolder.transform.position.y;
            
            Vector3 cameraPos = _camera.transform.position;
            cameraPos.x = centerX;
            cameraPos.y = centerY;
            _camera.transform.position = cameraPos;
        }
        
        public void AdaptCameraToBoard(int xDim, int yDim)
        {
            if (_camera == null)
                return;
            
            int originalX = BoardConfig?.XDim ?? 6;
            int originalY = BoardConfig?.YDim ?? 7;
            
            if (BoardConfig != null)
            {
                BoardConfig.XDim = xDim;
                BoardConfig.YDim = yDim;
            }
            
            AdaptCameraToBoard();
            
            if (BoardConfig != null)
            {
                BoardConfig.XDim = originalX;
                BoardConfig.YDim = originalY;
            }
        }
        

        public float GetRequiredCameraSize()
        {
            if (BoardConfig == null)
                return 7f;
                
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float aspectRatio = screenWidth / screenHeight;
            
            float boardWidth = BoardConfig.XDim;
            float boardHeight = BoardConfig.YDim;
            
            float targetWidth = boardWidth + _padding * 2;
            float targetHeight = boardHeight + _padding * 2;
            
            if (aspectRatio > 1f)
            {
                return targetHeight / 2f;
            }

            float targetWidthInUnits = targetWidth / aspectRatio;
            return targetWidthInUnits / 2f;
        }
        
        private BoardConfig BoardConfig => _levelService.GetCurrentLevelStaticData().boardConfig;
    }
}
