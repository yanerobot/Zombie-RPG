using UnityEngine;
using System;

namespace KK.Items
{
    public class UI_ItemSlotHandler : MonoBehaviour
    {
        [SerializeField] GameObject ui_slotPrefab;
        [SerializeField] UI_Slot[] ui_slots;

        public void FillUISlot(SlotInventoryItem slotItem, UI_Inventory ui_inventory, UI_InventoryItem ui_inventoryItem)
        {
            var ui_slot = GetUISlot(slotItem.slotType);

            ui_slot.icon = Instantiate(ui_slotPrefab, ui_slot.go.transform);
            var itemSlot = ui_slot.icon.GetComponent<UI_ItemSlot>();
            itemSlot.Init(slotItem, ui_inventory, ui_inventoryItem);
        }

        public void ClearUISlot(ItemSlot.SlotType slotType)
        {
            var ui_slot = GetUISlot(slotType);

            Destroy(ui_slot.icon);
            ui_slot.icon = null;
        }

        public UI_Slot GetUISlot(ItemSlot.SlotType slotType)
        {
            foreach (var ui_slot in ui_slots)
            {
                if (ui_slot.type == slotType)
                {
                    return ui_slot;
                }
            }

            Debug.LogError("Slot is null. Values should be assigned in the inspector");
            return null;
        }

        [Serializable]
        public class UI_Slot
        {
            public ItemSlot.SlotType type;
            public GameObject go;

            [NonSerialized]
            public GameObject icon;
        }
    }
}
