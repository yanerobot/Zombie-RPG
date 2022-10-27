using UnityEngine;
using UnityEngine.AI;
using KK.Utility;

namespace KK
{
    public class EnemyAI : StateMachine
    {
        //Add nav mesh movement, transition from any state
        [Header("Enemy AI")]
        public float searchingUpdateTime;

        [Header("Main")]
        [SerializeField] public EnemyStatsSO stats;

        [Header("References")]
        [SerializeField] public Animator animator;
        [SerializeField] public NavMeshAgent agent;

        [Header("Attack")]
        [SerializeField] AttackCollider[] attackColliders;
        public float AttackSequenceDelay => stats.recoveryAfterSequenceTime;

        public const int COMBAT_LAYER = 1;

        bool gotHit;

        public EnemyAIManager aiManager;

        float DistanceToTarget => Vector3.Distance(transform.position, aiManager.target.transform.position);


        void Start()
        {
            SetupStates();
        }

        void SetupStates()
        {
            var searching = new Searching(this);
            var running = new Running(this);
            var combat = new Combat(this);
            var getHit = new State("Get Hit", PlayHitAnimation);

            searching.AddTransition(running, IsTargetWithinSearchRange);
            running.AddTransition(combat, IsTargetWithinAttackRange);
            running.AddTransition(searching, () => !IsTargetWithinSearchRange());
            getHit.AddTransition(searching, () => !IsPlayingGetHitAnimation(), new TimeCondition(0.1f).HasTimePassed);
            combat.AddTransition(running, () => !IsTargetWithinAttackRange(), () => !IsPlayingAttackAnimation());
            combat.AddTransition(searching, IsTargetDead);

            AddAnyTransition(getHit, () => gotHit).AddTransitionCallBack(() => gotHit = false);

            SetState(searching);
        }

        void Update()
        {
            animator.SetFloat("MoveSpeed", Vector3.Distance(Vector3.zero, agent.velocity));
        }

        public void Die()
        {
            ExitState(immediate: true);

            foreach (var coll in attackColliders)
                coll.gameObject.SetActive(false);
        }

        public void GetHit()
        {
            gotHit = true;
        }

        public bool IsTargetDead()
        {
            return aiManager.target.isDead;
        }

        public void PlayHitAnimation()
        {
            animator.SetTrigger("GotHit");
        }


        public bool canRotate = true;

        public void LookAtTarget()
        {
            var direction = transform.position.Direction(aiManager.target.transform.position);
            transform.RotateTowards(direction.Where(y: 0), true, 4);
        }

        public void UpdatePreparationDurationMultiplier(float? multiplier = null)
        {
            if (multiplier == null)
                multiplier = (agent.stoppingDistance + stats.attackRange) / DistanceToTarget * 0.4f;

            animator.SetFloat("PreparationDurationMultiplier", (float)multiplier);
        }

        public void AEL_StartRegisterHit(AnimationEvent eventInfo)
        {
            canRotate = false;
            attackColliders[eventInfo.intParameter].gameObject.SetActive(true);
            attackColliders[eventInfo.intParameter].SetDamage(stats.baseDamage + Mathf.FloorToInt(eventInfo.floatParameter));
        }

        public void AEL_StopRegisterHit(int index)
        {
            canRotate = true;
            attackColliders[index].gameObject.SetActive(false);
        }
        public void AEL_SetAnimatorBool(AnimationEvent eventData)
        {
            bool value = false;

            if (eventData.intParameter == 0)
                value = false;
            else if (eventData.intParameter == 1)
                value = true;
            else
                Debug.LogError("This function should only recieve 0 or 1 as int parameter");

            animator.SetBool(eventData.stringParameter, value);
        }

        public void StopAttack()
        {
            animator.ResetTrigger("Attacked1");
            animator.ResetTrigger("Attacked2");
        }

        public bool IsPlayingAttackAnimation()
        {
            return animator.GetBool("IsAttacking");
        }
        public bool IsPlayingGetHitAnimation()
        {
            return animator.GetCurrentAnimatorStateInfo(0).IsName("Standing Hit");
        }

        public void Move()
        {
            if (!aiManager.IsTargetAvailible())
                return;

            var destination = GetBestDestination();

            agent.SetDestination(destination);
        }

        public void ToggleMovement(bool enabled)
        {
            print("Agent stopped: " + !enabled);

            agent.isStopped = !enabled;
        }

        Vector3 GetBestDestination()
        {
            var posOnCircle = aiManager.GetBestPositionAroundTarget(this);

            if (Vector3.Distance(aiManager.target.transform.position, transform.position) - 0.5f <= aiManager.radiusAroundTarget)
                return aiManager.target.transform.position;

            return posOnCircle;
        }

        public bool IsSwiping() => animator.GetBool("IsSwiping");


        public bool IsTargetWithinSearchRange()
        {
            return IsTargetWithinRange(stats.searchRange);
        }

        public bool IsTargetWithinAttackRange()
        {
            return IsTargetWithinRange(stats.attackRange);
        }

        bool IsTargetWithinRange(float range)
        {
            if (aiManager == null)
                return false;

            if (!aiManager.IsTargetAvailible())
                return false;

            if (DistanceToTarget >= range)
                return false;

            return true;
        }
    }
}