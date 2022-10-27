using UnityEngine;
using KK.Items;

namespace KK
{
    public class MeleeAttackStateResetter : StateMachineBehaviour
    {
        MeleeWeaponController meleeWeaponController;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.ResetTrigger(MeleeWeaponController.FAST_ATTACK);
            animator.ResetTrigger(MeleeWeaponController.HEAVY_ATTACK);
            animator.ResetTrigger(MeleeWeaponController.HOLD_ATTACK);
            animator.ResetTrigger(MeleeWeaponController.HEAVY_HOLD_ATTACK);

            if (meleeWeaponController == null)
                meleeWeaponController = animator.GetComponent<MeleeWeaponController>();

            meleeWeaponController.OnEndCombo();
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetNextAnimatorStateInfo(layerIndex).tagHash == Animator.StringToHash("Not equipped"))
                return;

            meleeWeaponController.OnStartCombo();
        }
    }
}
