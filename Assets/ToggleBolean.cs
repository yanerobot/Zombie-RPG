using UnityEngine;

namespace KK
{
    public class ToggleBolean : StateMachineBehaviour
    {
        public On setWhen;
        public string parameter;
        public bool flag;

        public enum On {
            Enter,
            Exit
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (setWhen == On.Enter)
            {
                animator.SetBool(parameter, flag);
            }
        }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (setWhen == On.Exit)
                animator.SetBool(parameter, flag);
        }

    }
}
