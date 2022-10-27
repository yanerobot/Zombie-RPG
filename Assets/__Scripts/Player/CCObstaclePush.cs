using UnityEngine;
using KK.Utility;

namespace KK
{
    [RequireComponent(typeof(CharacterController))]
    public class CCObstaclePush : MonoBehaviour
    {
        [SerializeField] float force;
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.collider.isTrigger)
                return;

            var rb = hit.collider.attachedRigidbody;

            if (rb == null)
                return;

            var direction = (hit.transform.position - transform.position).normalized;

            rb.AddForceAtPosition(direction * force, transform.position, ForceMode.Impulse);
        }
    }
}
