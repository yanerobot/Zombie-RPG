using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using TMPro;

namespace KK.Items
{
    public class UI_Inventory : MonoBehaviour, IOnLoadListener
    {
        [SerializeField] CanvasHandler canvasHandler;
        [SerializeField] UI_InventoryItem templatePrefab;
        [SerializeField] GameObject inventoryRootFolder;
        [SerializeField] UI_TabSystem tabSystem;
        [SerializeField] UI_ItemSlotHandler itemSlotHandler;
        [Header("Tooltip")]
        [SerializeField] Image toolTip;
        [SerializeField] Vector2 toolTipOffset;
        [SerializeField] TextMeshProUGUI displayNameGUI, descriptionGUI, additionalInfoGUI;
        [Header("Dropdown Menu")]
        [SerializeField] RectTransform dropDownMenu;
        [SerializeField] Vector2 dropDownOffset;
        [SerializeField] Button useButton, equipButton, unequipButton ,tossButton;
        [Header("Amount Input Field")]
        [SerializeField] AmountInputField amountInputField;

        List<(UI_InventoryItem, UI_InventoryItem)> uiItems;
        public InventorySystem inventory { get; private set; }

        InputMaster input;

        List<UI_InventoryItem> currentlyEquipped;

        UI_InventoryItem currentlySelected;

        public void OnLoad(MonoBehaviour caller)
        {
            uiItems = new List<(UI_InventoryItem, UI_InventoryItem)>();
            currentlyEquipped = new List<UI_InventoryItem>();

            input = new InputMaster();
            input.UI.OpenInventory.performed += ctx => ToggleInventory();

            caller.StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            GameObject player = null;

            while (player == null)
            {
                player = GameObject.FindWithTag("Player");
                yield return new WaitForSeconds(0.5f);
            }

            inventory = player.GetComponent<InventorySystem>();

            inventory.OnAdd.AddListener(Add);
            inventory.OnRemove.AddListener(Remove);
            inventory.OnStackChange.AddListener(RefreshStack);

            input.Enable();
        }

        void OnDestroy()
        {
            input.Dispose();

            if (inventory == null)
                return;

            inventory.OnAdd.RemoveListener(Add);
            inventory.OnRemove.RemoveListener(Remove);
            inventory.OnStackChange.RemoveListener(RefreshStack);

            equipButton.onClick.RemoveAllListeners();
            useButton.onClick.RemoveAllListeners();
            tossButton.onClick.RemoveAllListeners();
        }

        void ToggleInventory()
        {
            if (canvasHandler.IsFolderActive(inventoryRootFolder.name))
            {
                canvasHandler.Hide(inventoryRootFolder.name);
                SetActiveDropdown(false);
                DisableTooltip();
                Deselect();
            }
            else
            {
                canvasHandler.Show(inventoryRootFolder.name);
            }
        }

        public void Select(UI_InventoryItem uiItem)
        {
            if (uiItem != currentlySelected)
                Deselect();

            currentlySelected = uiItem;
        }

        public void Deselect()
        {
            if (currentlySelected == null)
                return;

            currentlySelected.Deselect();
            currentlySelected = null;
        }
        
        public void SetActiveDropdown(bool enabled)
        {
            if (amountInputField.IsInteracting)
                return;
            dropDownMenu.gameObject.SetActive(enabled);
        }

        public void ToggleDropDown(bool? enabled = null, Vector2 pos = default)
        {
            if (enabled == null)
            {
                enabled = !dropDownMenu.gameObject.activeSelf;
            }

            if ((bool)enabled)
            {
                dropDownMenu.position = pos + dropDownOffset;
            }
            SetActiveDropdown((bool)enabled);
        }

        void ToggleEquipUnequipButtons(bool isEquipped)
        {
            equipButton.gameObject.SetActive(!isEquipped);
            unequipButton.gameObject.SetActive(isEquipped);
        }

        public void EnableTooltip(Vector2 position, TooltipData data)
        {
            toolTip.gameObject.SetActive(true);
            toolTip.transform.position = position + toolTipOffset;
            displayNameGUI.text = data.displayName;
            descriptionGUI.text = data.description;
            additionalInfoGUI.text = data.additionalInfo;
        }
        
        public void DisableTooltip()
        {
            toolTip.gameObject.SetActive(false);
        }
        
        public void SetupButtons(UI_InventoryItem uiItem, bool isEquipped)
        {
            int stackSize = 1;
            equipButton.onClick.RemoveAllListeners();
            unequipButton.onClick.RemoveAllListeners();
            useButton.onClick.RemoveAllListeners();
            tossButton.onClick.RemoveAllListeners();

            if (uiItem.referenceItem is SlotInventoryItem slotItem)
            {
                ToggleEquipUnequipButtons(isEquipped);
                equipButton.onClick.AddListener(() => Equip(uiItem, slotItem));
                unequipButton.onClick.AddListener(() => Unequip(uiItem, slotItem));
            }
            else
            {
                equipButton.gameObject.SetActive(false);
                unequipButton.gameObject.SetActive(false);
            }

            if (uiItem.referenceItem is IUsable usable)
            {
                useButton.gameObject.SetActive(true);
                useButton.onClick.AddListener(usable.Use);
            }
            else
            {
                useButton.gameObject.SetActive(false);
            }

            if (uiItem.referenceItem is StackableInventoryItem stackable)
            {
                stackSize = stackable.stackSize;
            }

            amountInputField.Init(stackSize);
            tossButton.onClick.AddListener(() => Toss(uiItem));
        }

        void Toss(UI_InventoryItem uiItem)
        {
            if (uiItem.isEquipped)
            {
                uiItem.DeselectEquipped();
                currentlyEquipped.Remove(uiItem);
            }
            var isRemoved = inventory.Toss(uiItem.referenceItem, amountInputField.GetInputValue());
            if (isRemoved)
                Deselect();
        }

        public void Add(InventoryItem item)
        {
            var obj = tabSystem.GetTabBody(item);
            var uiElement = Instantiate(templatePrefab, obj.transform);
            var allUiElement = Instantiate(templatePrefab, tabSystem.AllItemsTab.body.transform);
            uiElement.Init(item, this);
            allUiElement.Init(item, this);
            uiItems.Add((uiElement, allUiElement));

        }

        void Equip(UI_InventoryItem uiItem, SlotInventoryItem slotItem)
        {
            if (inventory.Equip(slotItem))
            {
                itemSlotHandler.FillUISlot(slotItem, this, uiItem);
                currentlyEquipped.Add(uiItem);
                uiItem.SelectEquipped();
                ToggleEquipUnequipButtons(true);
            }
        }

        void Unequip(UI_InventoryItem uiItem, SlotInventoryItem slotItem)
        {
            inventory.Unequip(slotItem);

            itemSlotHandler.ClearUISlot(slotItem.slotType);
            currentlyEquipped.Remove(uiItem);
            uiItem.DeselectEquipped();
            ToggleEquipUnequipButtons(false);
        }

        public void Remove(InventoryItem item)
        {
            foreach (var uiItem in uiItems)
            {
                if (uiItem.Item1.referenceItem.id == item.id)
                {
                    Destroy(uiItem.Item1.gameObject);
                    Destroy(uiItem.Item2.gameObject);
                    uiItems.Remove(uiItem);
                    dropDownMenu.gameObject.SetActive(false);
                    return;
                }
            }
        }

        public void RefreshStack(InventoryItem item, int newStack)
        {
            foreach (var uiItem in uiItems)
            {
                if (uiItem.Item1.referenceItem.id == item.id)
                {
                    uiItem.Item1.SetStackText(newStack);
                    uiItem.Item2.SetStackText(newStack);
                    break;
                }
            }
        }
    }
}
