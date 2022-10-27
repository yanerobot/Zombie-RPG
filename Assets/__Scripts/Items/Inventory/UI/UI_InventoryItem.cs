using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;
using KK.Utility;

namespace KK.Items
{
    public class UI_InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler/*, IPointerClickHandler*/, IDragHandler
    {
        [SerializeField] TextMeshProUGUI stackText;
        [SerializeField] Image icon;
        [SerializeField] Image mainBg;
        [SerializeField] Shadow mainBGshadow;
        [SerializeField] GameObject selectionBG;
        [SerializeField] Color equippedColor;

        UI_Inventory inventoryUI;
        public InventoryItem referenceItem { get; private set; }

        TooltipData toolTipInfo;
        EventTrigger.TriggerEvent inventoryOnClick;

        Color originalColor;

        public bool isEquipped { get; private set; }

        bool isSelected;

        bool isInitialized;

        public void Init(InventoryItem item, UI_Inventory inventoryUI)
        {
            this.inventoryUI = inventoryUI;

            referenceItem = item;

            originalColor = mainBg.color;

            icon.sprite = item.icon;

            toolTipInfo = GetTooltipData();

            SetStackText(1);

            var eventTrigger = inventoryUI.GetComponent<EventTrigger>();
            inventoryOnClick = eventTrigger.triggers[0].callback;
            inventoryOnClick.AddListener(Deselect);

            isInitialized = true;
        }

        void OnDestroy()
        {
            if (inventoryOnClick == null)
                return;

            var eventCount = inventoryOnClick.GetPersistentEventCount();
            for (int i = 0; i < eventCount; i++)
            {
                print(inventoryOnClick.GetPersistentMethodName(i));
            }

            if (isInitialized)
                inventoryOnClick.RemoveListener(Deselect);
            
            eventCount = inventoryOnClick.GetPersistentEventCount();

            for (int i = 0; i < eventCount; i++)
            {
                print(inventoryOnClick.GetPersistentMethodName(i));
            }
        }

        void Deselect(BaseEventData _) => Deselect();

        public TooltipData GetTooltipData()
        {
            if (toolTipInfo != null)
                return toolTipInfo;

            string additionalInfo = "";
            if (referenceItem is SwordInventoryItem)
            {
                var sword = referenceItem as SwordInventoryItem;
                additionalInfo = $"Damage: {sword.damage} \nDurability: {sword.durability} \nWeight: {sword.weight}";
            }
            else if (referenceItem is HealthPotionInventoryItem)
            {
                var potion = referenceItem as HealthPotionInventoryItem;
                additionalInfo = $"Effect: {potion.effectAmount}";
            }

            return new TooltipData(referenceItem.displayName, referenceItem.description, additionalInfo);
        }

        public void SetStackText(int newStack)
        {
            if (newStack > 999)
                stackText.text = "999+";
            else if (newStack == 1)
                stackText.text = "";
            else
                stackText.text = newStack.ToString();
        }
        void Select()
        {
            selectionBG.SetActive(true);
            mainBGshadow.enabled = false;
            inventoryUI.Select(this);
            isSelected = true;
        }

        public void Deselect()
        {
            selectionBG.SetActive(false);
            if (!isEquipped)
                mainBGshadow.enabled = true;

            isSelected = false;
        }
        public void SelectEquipped()
        {
            mainBGshadow.enabled = false;
            mainBg.color = mainBg.color * equippedColor;
            isEquipped = true;
        }

        public void DeselectEquipped()
        {
            if (!isSelected)
                mainBGshadow.enabled = true;
            mainBg.color = originalColor;
            isEquipped = false;
        }

        #region Pointer Events
        //Self button event
        public void OnClickButton()
        {
            inventoryUI.SetupButtons(this, isEquipped);
            inventoryUI.ToggleDropDown(true, transform.position);
            Select();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            inventoryUI.EnableTooltip(transform.position, toolTipInfo);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.DisableTooltip();
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }
        #endregion
    }

    public class TooltipData
    {
        public string displayName, description, additionalInfo;
        public TooltipData(string displayName, string description, string additionalInfo)
        {
            this.displayName = displayName;
            this.description = description;
            this.additionalInfo = additionalInfo;
        }
    }
}
