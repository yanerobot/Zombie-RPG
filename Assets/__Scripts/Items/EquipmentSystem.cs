using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;
using System;
using KK.Utility;

namespace KK.Items
{
    [DefaultExecutionOrder(1)]
    public class EquipmentSystem : MonoBehaviour
    {
        public enum InputType
        {
            All,
            Equip,
            Toss
        }

        [SerializeField] public Animator animator;

        [SerializeField] float equipCooldown = 1;

        public MeleeWeaponController meleeWeaponController;
        

        public Transform rightHandHolder;
        public Transform sheathHolder;


        public List<ItemSlot> itemSlots;
        protected List<ItemObject> itemsToPick = new List<ItemObject>();

        public bool isEquipCooldown { get; private set; }

#if UNITY_EDITOR
        void OnValidate()
        {
            itemSlots = new List<ItemSlot>();
            foreach (ItemSlot.SlotType slotType in Enum.GetValues(typeof(ItemSlot.SlotType)))
            {
                itemSlots.Add(new ItemSlot(slotType));
            }
        }
#endif

        void OnEnable()
        {
            itemsToPick.Clear();
            //Сейчас он проверяет только правую руку, сделать так, чтобы проверял все айтемы и добавлял их в соответствующий слот
            //CheckItemHolder();
        }

        public bool Equip(ItemObject newItem)
        {
            if (isEquipCooldown || newItem == null) 
                return false;

            if (!TryGetSlot(newItem.inventoryItemData, out var slot))
            {
                Debug.LogError("Trying to equip item, but slot for the item doesn't exist");
                return false;
            }

            if (slot.itemObj != null)
                ClearSlot(slot);

            slot.itemObj = newItem;

            newItem.OnEquip(this);

            SetController(newItem);

            itemsToPick.Remove(newItem);

            StartEquipCooldown();

            return true;
        }

        public void ClearSlot(ItemSlot slot)
        {
            if (!itemSlots.Contains(slot))
            {
                Debug.LogError("Trying to toss item, but slot for the item doesn't exist");
                return;
            }

            UnsetController(slot.itemObj);

            slot.itemObj.OnToss();
            
            slot.itemObj = null;
        }

        public bool TryGetSlot(InventoryItem item, out ItemSlot slotVariable)
        {
            if (!(item is SlotInventoryItem slotItem))
            {
                slotVariable = null;
                return false;
            }

            foreach (var slot in itemSlots)
            {
                if (slotItem.slotType == slot.itemType)
                {
                    slotVariable = slot;
                    return true;
                }
            }

            Debug.LogError("Slot wasn't created for: " + slotItem.slotType);
            slotVariable = null;
            return false;
        }

        void StartEquipCooldown()
        {
            isEquipCooldown = true;
            this.Co_DelayedExecute(() => isEquipCooldown = false, equipCooldown);
        }

        void SetController(ItemObject item)
        {
            if (item is Melee)
                meleeWeaponController.SetController(item as Melee, rightHandHolder);
        }

        void UnsetController(ItemObject item)
        {
            if (item is Melee)
                meleeWeaponController.UnsetController();
        }

        public void SelectItem(ItemObject item)
        {
            if (itemsToPick.Contains(item))
                return;

            itemsToPick.Add(item);
        }

        public void DeselectItem(ItemObject item)
        {
            itemsToPick.Remove(item);
        }

        void CheckItemHolder()
        {
            var item = rightHandHolder.GetComponentInChildren<ItemObject>();
            if (item != null)
            {
                itemsToPick.Add(item);
                Equip(item);
            }
        }

        public ItemObject GetBestItem()
        {
            ItemObject bestItem = null;
            float bestScore = -Mathf.Infinity;
            foreach (var item in itemsToPick)
            {
                var dist = Vector3.Distance(transform.position, item.transform.position);
                var dir = (item.transform.position - transform.position).normalized;
                var dot = Vector3.Dot(transform.forward, dir); 

                var currentScore = 1 / dist * dot;
                if (currentScore > bestScore)
                {
                    bestItem = item;
                    bestScore = currentScore;
                }
            }

            return bestItem;
        }

        public virtual void FreezeEquipment(InputType action = InputType.All) { }

        public virtual void UnFreezeEquipment(InputType action = InputType.All) { }

        protected virtual void OnUse(InputType actionType) { }
    }

    [Serializable]
    public class ItemSlot
    {
        public SlotType itemType;
        public Transform itemHolder;
        [NonSerialized]
        public ItemObject itemObj;

        public enum SlotType
        {
            helmet,
            necklace,
            shoulder, 
            chest, 
            cloak, 
            gloves, 
            belt, 
            pants, 
            boots, 
            ring, 
            weapon1, 
            weapon2, 
            ranged
        }

        public ItemSlot(SlotType type)
        {
            itemType = type;
        }
    }
}