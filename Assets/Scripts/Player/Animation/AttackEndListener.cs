using Character;
using UnityEngine;

namespace Player.Animation
{
    public class AttackEndListener : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var playerView = animator.GetComponent<CharacterAnimationView>();
            playerView.OnAttackAnimationFinished();
        }
    }
}