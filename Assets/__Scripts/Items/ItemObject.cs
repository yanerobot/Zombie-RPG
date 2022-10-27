using UnityEngine;

namespace KK.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class ItemObject : MonoBehaviour
    {
        [SerializeField] Vector3 holdingPositionOffset, holdingRotationOffset;
        [SerializeField] float tossForce, tossAngularVelocity;
        [SerializeField] Transform initialParent;
        [SerializeField] Rigidbody mainRb;

        public InventoryItem inventoryItemData;

        protected EquipmentSystem CurrentHolder { get; private set; }


        Collider[] colliders;

        void Awake()
        {
            colliders = GetComponentsInChildren<Collider>();
        }

        void DisableColliders()
        {
            foreach (var coll in colliders)
                coll.enabled = false;
        }

        void EnableColliders()
        {
            foreach (var coll in colliders)
                coll.enabled = true;
        }

        public virtual void OnEquip(EquipmentSystem system)
        {
            mainRb.isKinematic = true;

            DisableColliders();

            CurrentHolder = system;

            SetHoldingTransformSettings();
        }

        public void OnEquip(Transform holder)
        {
            
        }

        public virtual void OnToss()
        {
            mainRb.isKinematic = false;

            EnableColliders();

            transform.SetParent(initialParent);
            transform.rotation = Quaternion.Euler(new Vector3(1, CurrentHolder.transform.eulerAngles.y, 1)) * Quaternion.identity;

            mainRb.AddForce(CurrentHolder.transform.forward * tossForce, ForceMode.Impulse);
            mainRb.angularVelocity = CurrentHolder.transform.forward * tossAngularVelocity;

            CurrentHolder = null;
        }

        protected virtual void SetHoldingTransformSettings()
        {
            SetRightHandTransformSettings();
        }

        public void SetRightHandTransformSettings()
        {
            PositionItemOnHolder(CurrentHolder.rightHandHolder);
        }

        public void PositionItemOnHolder(Transform holder)
        {
            transform.SetParent(holder);
            transform.rotation = holder.rotation;
            transform.localPosition = holdingPositionOffset;
            transform.localEulerAngles += holdingRotationOffset;
        }
    }
}