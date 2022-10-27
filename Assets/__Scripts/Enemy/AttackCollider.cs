using UnityEngine;
using KK.Utility;

namespace KK
{
    [RequireComponent(typeof(Collider))]
    public class AttackCollider : MonoBehaviour
    {
        [SerializeField] LayerMask opposingLayers;
        [SerializeField] EnemyStatsSO enemyStatsSO;

        int damage;

        void Awake()
        {
            gameObject.SetActive(false);
        }

        public void SetDamage(int value)
        {
            damage = value;
        }

        void OnTriggerEnter(Collider other)
        {
            if (opposingLayers.Contains(other.gameObject.layer) &&  other.TryGetComponent(out Health health))
            {
                health.TakeDamage(damage);
            }
        }
    }
}
