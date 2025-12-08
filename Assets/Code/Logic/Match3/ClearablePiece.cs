using System.Collections;
using Code.Services.LevelConductors.Locator;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Code.Logic.Match3
{
    public class ClearablePiece : MonoBehaviour
    {
        [FormerlySerializedAs("clearAnimation")] 
        [SerializeField] public AnimationClip _clearAnimation;
        [SerializeField] protected GamePieceView _pieceView;
        [SerializeField] private Animator _animator;

        private ILevelServiceLocator _levelServiceLocator;
        
        [Inject]
        private void Constructor(ILevelServiceLocator levelServiceLocator)
        {
            _levelServiceLocator = levelServiceLocator;
        }

        private void OnValidate()
        {
            if (_pieceView == null)
            {
                _pieceView = GetComponent<GamePieceView>();
            }

            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
        }

        public bool IsBeingCleared { get; private set; }
        
        public virtual void Clear()
        {
            if (_pieceView?.Data != null)
            {
                _levelServiceLocator.GetForCurrentLevel().OnPieceCleared(_pieceView);
            }
            
            IsBeingCleared = true;
            StartCoroutine(ClearCoroutine());
        }

        private IEnumerator ClearCoroutine()
        {
            if (_animator != null)
            {
                _animator.Play(_clearAnimation.name);

                yield return new WaitForSeconds(_clearAnimation.length);

                Destroy(gameObject);
            }
        }
    }
}
