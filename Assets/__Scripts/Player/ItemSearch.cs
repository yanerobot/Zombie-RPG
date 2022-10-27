using UnityEngine;

namespace KK.Items
{
    [RequireComponent(typeof(Collider))]
    public class ItemSearch : MonoBehaviour
    {
        [SerializeField] EquipmentSystem equipmentSystem;

        void Awake()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ItemObject item))
            {
                equipmentSystem.SelectItem(item);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ItemObject item))
            {
                equipmentSystem.DeselectItem(item);
            }
        }
    } 
}