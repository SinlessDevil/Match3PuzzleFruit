using System.Collections;
using UnityEngine;

namespace Code.Logic.Match3
{
    public class MovablePiece : MonoBehaviour
    {
        private GamePiece _piece;
        private IEnumerator _moveCoroutine;

        private void Awake()
        {
            _piece = GetComponent<GamePiece>();
        }

        public void Move(int newX, int newY, float time)
        {
            Vector2 endPosition;
            
            if (_piece.MatchBoardControllerRef != null)
            {
                endPosition = _piece.MatchBoardControllerRef.GetWorldPosition(newX, newY);
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
            _piece.X = newX;
            _piece.Y = newY;

            Vector3 startPos = transform.position;
            Vector3 endPos = endPosition;

            for (float t = 0; t <= 1 * time; t += Time.deltaTime)
            {
                _piece.transform.position = Vector3.Lerp(startPos, endPos, t / time);
                yield return null;
            }

            _piece.transform.position = endPos;
        }
    }
}
