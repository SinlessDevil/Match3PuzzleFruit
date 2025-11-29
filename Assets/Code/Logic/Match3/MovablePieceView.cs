using System.Collections;
using UnityEngine;

namespace Code.Logic.Match3
{
    public class MovablePieceView : MonoBehaviour
    {
        private GamePieceView _pieceView;
        private IEnumerator _moveCoroutine;

        private void Awake()
        {
            _pieceView = GetComponent<GamePieceView>();
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
            if (_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }

            _moveCoroutine = MoveCoroutine(newX, newY, endPosition, time);
            StartCoroutine(_moveCoroutine);
        }

        private IEnumerator MoveCoroutine(int newX, int newY, Vector2 endPosition, float time)
        {
            if (_pieceView?.Data != null)
            {
                _pieceView.Data.SetPosition(newX, newY);
            }

            Vector3 startPos = transform.position;
            Vector3 endPos = endPosition;

            for (float t = 0; t <= 1 * time; t += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(startPos, endPos, t / time);
                yield return null;
            }

            transform.position = endPos;
        }
    }
}

