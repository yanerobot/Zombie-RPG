using UnityEngine;
using System.Collections.Generic;

namespace KK
{
    public class HideVisionObstacles : MonoBehaviour
    {
        List<Transform> currentlyHidden;

        void Awake()
        {
            currentlyHidden = new List<Transform>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Hideble hideble))
            {
                hideble.Hide();
                currentlyHidden.Add(hideble.transform);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (currentlyHidden.Contains(other.transform))
            {
                other.GetComponent<Hideble>().Show();
                currentlyHidden.Remove(other.transform);
            }
        }
    }
}
