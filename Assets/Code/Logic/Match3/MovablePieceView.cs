using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Logic.Match3
{
    public class MovablePieceView : MonoBehaviour
    {
        private GamePieceView _pieceView;
        private CancellationTokenSource _moveCancellationTokenSource;

        private void Awake()
        {
            _pieceView = GetComponent<GamePieceView>();
        }

        private void OnDestroy()
        {
            _moveCancellationTokenSource?.Cancel();
            _moveCancellationTokenSource?.Dispose();
            _moveCancellationTokenSource = null;
        }

        public void Move(int newX, int newY, float time)
        {
            Vector2 endPosition;
            
            if (_pieceView?.Data?.MatchBoardController != null)
            {
                endPosition = _pieceView.Data.MatchBoardController.GetWorldPosition(newX, newY);
            }
            else
            {
                endPosition = transform.position;
            }
            
            Move(newX, newY, endPosition, time);
        }
        
        public void Move(int newX, int newY, Vector2 endPosition, float time)
        {
            _moveCancellationTokenSource?.Cancel();
            _moveCancellationTokenSource?.Dispose();
            _moveCancellationTokenSource = new CancellationTokenSource();

            MoveAsync(newX, newY, endPosition, time, _moveCancellationTokenSource.Token).Forget();
        }

        private async UniTaskVoid MoveAsync(int newX, int newY, Vector2 endPosition, float time, CancellationToken cancellationToken)
        {
            if (_pieceView?.Data != null)
            {
                _pieceView.Data.SetPosition(newX, newY);
            }

            Vector3 startPos = transform.position;
            Vector3 endPos = endPosition;
            float elapsedTime = 0f;

            while (elapsedTime < time && !cancellationToken.IsCancellationRequested)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / time);
                transform.position = Vector3.Lerp(startPos, endPos, t);
                await UniTask.Yield(cancellationToken);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                transform.position = endPos;
            }
        }
    }
}

