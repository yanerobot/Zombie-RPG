using UnityEngine;
using System.Collections.Generic;
using System;

namespace KK.Items
{
    public class PreviewCharacterGearShow : MonoBehaviour
    {
        List<ItemSlot> previewItemSlots;
        void Awake()
        {
            previewItemSlots = new List<ItemSlot>();

            foreach (ItemSlot.SlotType slotType in Enum.GetValues(typeof(ItemSlot.SlotType)))
            {
                previewItemSlots.Add(new ItemSlot(slotType));
            }
        }

        public void DuplicateItem(InventoryItem newItem)
        {
            var itemGo = Instantiate(newItem.prefab, transform);
            var item = itemGo.GetComponent<ItemObject>();

            //item.OnEquip()
        }

        void ClearSlot(ItemSlot slot)
        {
            if (!previewItemSlots.Contains(slot))
            {
                Debug.LogError("Trying to toss item, but slot for the item doesn't exist");
                return;
            }

            slot.itemObj.OnToss();

            slot.itemObj = null;
        }
    }
}
