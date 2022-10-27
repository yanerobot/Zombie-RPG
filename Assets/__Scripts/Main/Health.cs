using UnityEngine;
using UnityEngine.Events;

namespace KK
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float InvulnerabilityAfterHit;
        public int maxHealth;
        public int currentHealth { get; private set; }

        public bool isDead { get; private set; }

        [Tooltip("Called even if target is dead")]
        public UnityEvent _OnRegisterHit;
        public UnityEvent<int> _OnDamage, _OnRestore;
        public UnityEvent _OnDie;

        bool invulnerable;
        float timeAfterFirstHit;
        bool startCountingTime;

        void OnEnable()
        {
            Init();
        }

        void Update()
        {
            if (startCountingTime)
            {
                if (timeAfterFirstHit >= InvulnerabilityAfterHit)
                {
                    startCountingTime = false;
                    invulnerable = false;
                }
                else
                {
                    timeAfterFirstHit += Time.deltaTime;
                }
            }
        }

        public virtual void Init()
        {
            currentHealth = maxHealth;
            isDead = false;
        }
#if UNITY_EDITOR
        [ContextMenu("Take Damage")]
        public void TakeDamageTest()
        {
            TakeDamage(10);
        }
        [ContextMenu("Kill")]
        public void Kill()
        {
            TakeDamage(maxHealth);
        }
#endif

        public void TakeDamage(int amount)
        {
            _OnRegisterHit?.Invoke();

            if (isDead) 
                return;

            if (invulnerable)
                return;

            timeAfterFirstHit = 0;
            startCountingTime = true;
            invulnerable = true;

            currentHealth -= amount;

            if (currentHealth < 0) 
                currentHealth = 0;


            if (currentHealth == 0)
            {
                isDead = true;
                _OnDie?.Invoke();
                return;
            }

            _OnDamage?.Invoke(currentHealth);
        }
        public void Restore(int amount)
        {
            currentHealth += amount;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            _OnRestore?.Invoke(currentHealth);
        }
    }
}
