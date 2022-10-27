using KK.Items;
using KK.Utility;
using UnityEngine;

namespace KK.Player
{
    public class PlayerEquipmentSystem : EquipmentSystem
    {
        [Header("Player")]
        [SerializeField] InventorySystem inventory;
        InputMaster input;
        void Awake()
        {
            input = new InputMaster();
            input.Items.Enable();

            input.Items.Equip.performed += _ => AddToInventory();

        }

        void OnDestroy()
        {
            input.Dispose();
        }

        public void OnDie()
        {
            FreezeEquipment();
        }

        public void AddToInventory()
        {
            var item = GetBestItem();

            if (item == null)
                return;

            inventory.Add(item.inventoryItemData);
            itemsToPick.Remove(item);
            print($"Removing {item}");
            Destroy(item.gameObject);
        }

        public override void FreezeEquipment(InputType action = InputType.All)
        {
            switch (action)
            {
                case InputType.All:
                    input.Items.Disable();
                    break;
                case InputType.Equip:
                    input.Items.Equip.Disable();
                    break;
                case InputType.Toss:
                    input.Items.Toss.Disable();
                    break;
                default:
                    break;
            }
        }
        public override void UnFreezeEquipment(InputType action = InputType.All)
        {
            switch (action)
            {
                case InputType.All:
                    input.Items.Enable();
                    break;
                case InputType.Equip:
                    input.Items.Equip.Enable();
                    break;
                case InputType.Toss:
                    input.Items.Toss.Enable();
                    break;
                default:
                    break;
            }
        }
    }
}