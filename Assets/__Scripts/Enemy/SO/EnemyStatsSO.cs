using UnityEngine;

namespace KK
{
    [CreateAssetMenu(fileName = "EnemyStats", menuName = "SO/Enemies/EnemyStats")]
    public class EnemyStatsSO : ScriptableObject
    {
        [Header("Attack")]
        [Tooltip("Additive to agent's stopping distance")] public float attackRange;
        public float attackDelay;
        public int hitsToUnfreeze;
        public LayerMask damagableMask;
        public int baseDamage;
        public float recoveryAfterSequenceTime;

        [Header("Search")]
        public float searchRange;
        public float innerCircleRadius;
        public float outerCircleRadius;

        public string[] attackSequences;

        public string RandomAttackSequence => attackSequences[Random.Range(0, attackSequences.Length)];
    }
}
