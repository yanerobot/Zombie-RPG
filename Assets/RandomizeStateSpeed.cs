using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KK
{
    public class RandomizeStateSpeed : StateMachineBehaviour
    {
        [SerializeField] float min, max;

        float initial;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            initial = animator.speed;
            animator.speed = Random.Range(min, max);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.speed = initial;
        }
    }
}
