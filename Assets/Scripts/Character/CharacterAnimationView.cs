using R3;
using UnityEngine;

namespace Character
{
    public class CharacterAnimationView : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        public Observable<Unit> OnAnimationFinishedAsObservable => _attackAnimationFinishedSubject;
        private readonly Subject<Unit> _attackAnimationFinishedSubject = new();

        public Animator Animator => animator;

        public void OnAttackAnimationFinished()
        {
            _attackAnimationFinishedSubject.OnNext(Unit.Default);
        }
    }
}