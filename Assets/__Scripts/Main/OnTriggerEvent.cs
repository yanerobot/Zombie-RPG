using UnityEngine;
using UnityEngine.Events;

namespace KK
{
    public class OnTriggerEvent : MonoBehaviour
    {
        [SerializeField] UnityEvent action;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                action.Invoke();
            }
        }
    }
}
