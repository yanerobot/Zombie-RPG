using UnityEngine;
using KK.Utility;

namespace KK.Items
{
    public class Melee : ItemObject
    {
        [Header("Melee")]
        [SerializeField] int damage;
        [SerializeField] Vector3 sheathPositionOffset, sheathRotationOffset;
        [SerializeField] BladeController bladeController;

        public override void OnEquip(EquipmentSystem system)
        {
            base.OnEquip(system);
            bladeController.SetWeaponController(CurrentHolder.meleeWeaponController);
        }

        public override void OnToss()
        {
            base.OnToss();
            bladeController.UnsetWeaponControler();
        }

        public void OnStartSwing(int damage)
        {
            bladeController.gameObject.SetActive(true);
            bladeController.StartRegister(damage);
        }
        
        public void OnEndSwing()
        {
            bladeController.gameObject.SetActive(false);
            bladeController.StopRegister();
        }

        protected override void SetHoldingTransformSettings()
        {
            transform.SetParent(CurrentHolder.sheathHolder);
            transform.rotation = CurrentHolder.sheathHolder.rotation;
            transform.localPosition = sheathPositionOffset;
            transform.localEulerAngles += sheathRotationOffset;
        }
    }
}