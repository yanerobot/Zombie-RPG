using UnityEngine;

namespace KK.Items
{
    [CreateAssetMenu(menuName = "SO/Inventory/Items/Potion")]
    public class HealthPotionInventoryItem : StackableInventoryItem, IUsable
    {
        public int effectAmount;

        public void Use()
        {
            holder.TryGetComponent(out Health health);
            if (health == null)
            {
                Debug.LogWarning("Health component was not found");
                return;
            }
            health.Restore(effectAmount);
        }
    }
}
