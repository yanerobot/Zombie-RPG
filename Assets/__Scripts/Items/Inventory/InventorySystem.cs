using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using KK.Utility;

namespace KK.Items
{
    public class InventorySystem : MonoBehaviour
    {
        public EquipmentSystem es;
        [HideInInspector]
        public UnityEvent<InventoryItem> OnAdd, OnRemove;
        [HideInInspector]
        public UnityEvent<InventoryItem, int> OnStackChange;
        List<InventoryItem> allItems;

        void Awake()
        {
            allItems = new List<InventoryItem>();
        }

        public void Add(InventoryItem newItem)
        {
            if (newItem is StackableInventoryItem)
            {
                foreach (var item in allItems)
                {
                    if (item.GetType() == newItem.GetType())
                    {
                        var stackable = item as StackableInventoryItem;
                        stackable.AddToStack();
                        OnStackChange?.Invoke(stackable, stackable.stackSize);
                        return;
                    }
                }
            }

            newItem.OnEquip(gameObject);

            allItems.Add(newItem);
            OnAdd?.Invoke(newItem);
        }

        public bool Remove(InventoryItem newItem)
        {
            if (!allItems.Contains(newItem))
            {
                Debug.LogError($"{newItem} is not on the list but trying to remove!");
                return false;
            }

            if (newItem is StackableInventoryItem)
            {
                var stackable = newItem as StackableInventoryItem;

                if (stackable.stackSize > 1)
                {
                    stackable.RemoveFromStack();
                    OnStackChange?.Invoke(stackable, stackable.stackSize);
                    return false;
                }
            }

            newItem.OnToss();

            allItems.Remove(newItem);
            OnRemove?.Invoke(newItem);
            return true;
        }

        public bool Equip(SlotInventoryItem newItem)
        {
            var go = Instantiate(newItem.prefab);
            var item = go.GetComponent<ItemObject>();

            if (item == null || !es.Equip(item))
            {
                Destroy(go);
                return false;
            }
            
            return true;
        }

        public void Unequip(SlotInventoryItem slotItem)
        {
            if (!es.TryGetSlot(slotItem, out var slot))
            {
                Debug.LogWarning($"{slotItem} is not equipped, but unequip button was pressed (?)");
                return;
            }

            Destroy(slot.itemObj.gameObject);
            es.ClearSlot(slot);
        }

        public bool Toss(InventoryItem item, int amount)
        {
            es.TryGetSlot(item, out var slot);

            bool isRemoved = false;

            for (int i = 0; i < amount; i++)
            {
                if (slot != null && slot.itemObj != null)
                {
                    es.ClearSlot(slot);
                }
                else
                {
                    Instantiate(item.prefab, transform.position.AddTo(y: 1) + transform.forward * 2, Random.rotation);
                }
                isRemoved = Remove(item);
            }

            return isRemoved;
        }
    }
}
