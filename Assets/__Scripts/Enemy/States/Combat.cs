using UnityEngine;

namespace KK
{
    public class Combat : State
    {
        bool hasAttacked;
        EnemyAI AI;

        string attackSequence;

        int sequenceIndex = 0;

        float recoveryTime;

        public Combat(StateMachine fsm)
        {
            AI = fsm as EnemyAI;
        }

        public override void OnEnter()
        {
            sequenceIndex = 0;
            AI.SetUpdateMode(StateMachine.UpdateMode.Update);
            AI.agent.updateRotation = false;

            attackSequence = AI.stats.RandomAttackSequence;

            if (!int.TryParse(attackSequence, out var _))
                Debug.LogError("Wrong sequence format");
        }
        public override void OnUpdate()
        {
            if (AI.canRotate)
                AI.LookAtTarget();

            if (AI.aiManager.targetWeaponController.missedAttack)
            {
                int attackNumber = 1;
                hasAttacked = true;
                AI.animator.SetTrigger("Attacked" + attackNumber);
                AI.UpdatePreparationDurationMultiplier(0.3f);
                AI.animator.SetBool("IsFirstAttack", false);
                AI.animator.SetBool("IsAttacking", true);
                AI.aiManager.targetWeaponController.missedAttack = false;
                return;
            }

            if (sequenceIndex >= attackSequence.Length)
            {
                recoveryTime += Time.deltaTime;

                if (recoveryTime >= AI.AttackSequenceDelay)
                {
                    attackSequence = AI.stats.RandomAttackSequence;
                    recoveryTime = 0;
                    sequenceIndex = 0;
                }

                return;
            }

            AI.UpdatePreparationDurationMultiplier();

            if (!AI.IsTargetWithinAttackRange())
            {
                if (AI.animator.GetBool("IsPreparing"))
                {
                    AI.animator.SetTrigger("CancelAttack");
                }
                return;
            }

            if (AI.animator.GetBool("IsFirstAttack") || 
                (AI.animator.GetBool("IsRecovering") && !hasAttacked))
            {
                int attackNumber = int.Parse(attackSequence[sequenceIndex].ToString());
                hasAttacked = true;
                AI.animator.SetTrigger("Attacked" + attackNumber);
                AI.animator.SetBool("IsFirstAttack", false);
                AI.animator.SetBool("IsAttacking", true);
                sequenceIndex++;
            }
            else if (!AI.animator.GetBool("IsRecovering"))
            {
                hasAttacked = false;
            }
        }

        /*
         и так
            сделать отпрыг назад
            отпрыг будет срабатывать когда игрок подбирается слишком близко к мобу

        */

        public override void OnExit()
        {
            AI.SetUpdateMode(default);
            AI.StopAttack();
            AI.agent.updateRotation = true;
        }
    }
}
