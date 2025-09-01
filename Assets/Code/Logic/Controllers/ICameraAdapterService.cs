using Code.Logic.Holders;

namespace Code.Logic.Controllers
{
    public interface ICameraAdapterService
    {
        void Initialize(MapHolder mapHolder);
        void Dispose();
        void AdaptCameraToBoard();
        void AdaptCameraToBoard(int xDim, int yDim);
        void CenterCameraOnBoard();
        float GetRequiredCameraSize();
    }
}