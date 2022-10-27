using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace KK.Items
{
    public class UI_ItemSlot : MonoBehaviour, IDragHandler
    {
        [SerializeField] Image iconImage;

        UI_Inventory ui_inventory;
        UI_InventoryItem ui_InventoryItem;
        public void Init(SlotInventoryItem slotItem, UI_Inventory ui_inventory, UI_InventoryItem ui_InventoryItem)
        {
            this.ui_inventory = ui_inventory;
            this.ui_InventoryItem = ui_InventoryItem;

            iconImage.sprite = slotItem.icon;
        }

        public void ShowInfo()
        {
            ui_inventory.EnableTooltip(transform.position, ui_InventoryItem.GetTooltipData());
        }

        public void HideInfo()
        {
            ui_inventory.DisableTooltip();
        }

        public void ShowDropdown()
        {
            ui_inventory.ToggleDropDown(true, transform.position);
        }

        public void HideDropdown()
        {
            ui_inventory.ToggleDropDown(false);
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }
    }
}
