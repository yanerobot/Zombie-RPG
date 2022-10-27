using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using KK.Items;
using DG.Tweening;
using TMPro;

namespace KK
{
    public class UI_TabSystem : MonoBehaviour
    {
        [SerializeField] GameObject equipmentSubBody;
        [SerializeField] Tab[] tabs;

        Tab activeTab = null;

        public Tab AllItemsTab;

#if UNITY_EDITOR
        void OnValidate()
        {
            Init();
        }
#endif

        void Awake()
        {
            activeTab = null;
            Init();
            foreach (var tab in tabs)
            {
                tab.Init();
                if (!tab.button.interactable && activeTab == null)
                    SelectTab(tab);
                else
                    tab.button.interactable = true;
            }
        }

        void Init()
        {
            if (tabs.Length != transform.childCount)
            {
                Debug.LogWarning("Childs and tabs should match!");
                return;
            }

            var i = 0;
            foreach (Transform tr in transform)
            {
                tabs[i].button = tr.GetComponent<Button>();
                i++;
            }
        }

        public void SelectOne(Button selectedButton)
        {
            var tab = GetTabByButton(selectedButton);

            DeselectActiveTab();

            SelectTab(tab);

            activeTab = tab;
        }

        void SelectTab(Tab tab)
        {
            tab.button.interactable = false;
            tab.body.gameObject.SetActive(true);
            equipmentSubBody.gameObject.SetActive(tab.showEquipmentSubBody);
            tab.buttonTextMesh.fontWeight = FontWeight.Bold;

            DeselectActiveTab();

            activeTab = tab;
        }

        void DeselectActiveTab()
        {
            if (activeTab == null)
                return;

            activeTab.buttonTextMesh.fontWeight = FontWeight.Regular;
            activeTab.button.interactable = true;
            activeTab.body.gameObject.SetActive(false);
        }

        Tab GetTabByButton(Button button)
        {
            foreach (var tab in tabs)
            {
                if (tab.button == button)
                    return tab;
            }
            return null;
        }

        public GameObject GetTabBody(InventoryItem item)
        {
            foreach (var tab in tabs)
            {
                if (tab.itemType == item.tabType)
                    return tab.body.gameObject;
            }
            return null;
        }

        [Serializable]
        public class Tab
        {
            public InventoryItem.ItemType itemType;
            public Button button;
            public GameObject body;
            public bool showEquipmentSubBody;
            [NonSerialized]
            public RectTransform bodyRectTransform;
            [NonSerialized]
            public LayoutElement bodyLayoutElement;
            [NonSerialized]
            public TextMeshProUGUI buttonTextMesh;

            public void Init()
            {
                bodyRectTransform = body.GetComponent<RectTransform>();
                bodyLayoutElement = body.GetComponent<LayoutElement>();
                buttonTextMesh = button.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }
}
