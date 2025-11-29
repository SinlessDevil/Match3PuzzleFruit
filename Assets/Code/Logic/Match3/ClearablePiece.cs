using System.Collections;
using Code.Services.LevelConductors.Locator;
using UnityEngine;
using Zenject;

namespace Code.Logic.Match3
{
    public class ClearablePiece : MonoBehaviour
    {
        public AnimationClip clearAnimation;
        
        protected GamePieceView _pieceView;

        private ILevelServiceLocator _levelServiceLocator;
        
        [Inject]
        private void Constructor(ILevelServiceLocator levelServiceLocator)
        {
            _levelServiceLocator = levelServiceLocator;
        }
        
        private void Awake()
        {
            _pieceView = GetComponent<GamePieceView>();
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
            Animator animator = GetComponent<Animator>();

            if (animator != null)
            {
                animator.Play(clearAnimation.name);

                yield return new WaitForSeconds(clearAnimation.length);

                Destroy(gameObject);
            }
        }
    }
}
