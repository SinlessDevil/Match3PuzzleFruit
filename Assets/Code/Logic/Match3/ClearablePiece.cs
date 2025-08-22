using System.Collections;
using Code.Services.LevelConductors.Locator;
using UnityEngine;
using Zenject;

namespace Match3
{
    public class ClearablePiece : MonoBehaviour
    {
        public AnimationClip clearAnimation;
        
        protected GamePiece _piece;

        private ILevelServiceLocator _levelServiceLocator;
        
        [Inject]
        private void Constructor(ILevelServiceLocator levelServiceLocator)
        {
            _levelServiceLocator = levelServiceLocator;
        }
        
        private void Awake()
        {
            _piece = GetComponent<GamePiece>();
        }

        public bool IsBeingCleared { get; private set; }
        
        public virtual void Clear()
        {
            _levelServiceLocator.GetForCurrentLevel().OnPieceCleared(_piece);
            
            IsBeingCleared = true;
            StartCoroutine(ClearCoroutine());
        }

        private IEnumerator ClearCoroutine()
        {
            var animator = GetComponent<Animator>();

            if (animator)
            {
                animator.Play(clearAnimation.name);

                yield return new WaitForSeconds(clearAnimation.length);

                Destroy(gameObject);
            }
        }
    }
}
