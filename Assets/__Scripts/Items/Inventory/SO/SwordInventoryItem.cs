using UnityEngine;

namespace KK.Items
{
    [CreateAssetMenu(menuName = "SO/Inventory/Items/Sword")]
    public class SwordInventoryItem : SlotInventoryItem
    {
        [Header("Sword")]
        public float damage;
        public float weight;
        public float durability = 100;
    }
}
