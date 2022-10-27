using UnityEngine;
using System;

namespace KK.Items
{
    public abstract class InventoryItem : ScriptableObject
    {
        public enum ItemType
        {
            All,
            Weapon,
            Armor,
            Quest,
            Consumable,
            Other
        }

        static int lastId = 0;


        [Header("Inventory Item")]
        public ItemType tabType;
        public int id;
        public string displayName;
        public Sprite icon;
        public GameObject prefab;

        [TextArea(5, 20)]
        public string description;
        protected GameObject holder;

        void Awake()
        {
            id = lastId + 1;
            lastId = id;
        }

        public void OnEquip(GameObject newHolder)
        {
            holder = newHolder;
        }

        public void OnToss()
        {
            holder = null;
        }
    }

    public abstract class StackableInventoryItem : InventoryItem
    {
        public int stackSize { get; private set; } = 1;

        void OnEnable()
        {
            stackSize = 1;
        }

        public void AddToStack()
        {
            stackSize++;
        }

        public void RemoveFromStack()
        {
            stackSize--;
        }
    }

    public abstract class SlotInventoryItem: InventoryItem
    {
        [Header("Slot data")]
        public ItemSlot.SlotType slotType;
    }

    public interface IUsable
    {
        public void Use();
    }
}
