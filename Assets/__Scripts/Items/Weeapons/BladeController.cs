using UnityEngine;
using UnityEngine.Events;
using KK.Utility;

namespace KK
{
    public class BladeController : MonoBehaviour
    {
        [SerializeField] TrailRenderer tr;
        MeleeWeaponController meleeController;
        int damage;

        bool dealtDamage;

        void OnTriggerEnter(Collider other)
        {
            if (meleeController.opposingLayers.Contains(other.gameObject.layer) &&
                other.TryGetComponent(out Health health))
            {
                health.TakeDamage(Mathf.RoundToInt(damage));
                dealtDamage = true;
            }
        }

        public void SetWeaponController(MeleeWeaponController controller)
        {
            meleeController = controller;
        }

        public void UnsetWeaponControler()
        {
            meleeController = null;
        }

        public void StartRegister(int damage)
        {
            dealtDamage = false;
            enabled = true;
            tr.enabled = true;
            this.damage = damage;
        }

        public void StopRegister()
        {
            tr.enabled = false;
            enabled = false;
            if (dealtDamage == false)
            {
                meleeController.OnAttackMiss?.Invoke();
                meleeController.missedAttack = true;
            }
        }
    }
}
