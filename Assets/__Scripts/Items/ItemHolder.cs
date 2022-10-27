using UnityEngine;

namespace KK.Items
{
    public class ItemHolder
    {
        public Transform transform;
        public EquipmentSystem ES;
        public CCIsoMovement3D Movement;
        public Animator animator;

        public ItemHolder(EquipmentSystem es = null, CCIsoMovement3D movement = null, Animator animator = null)
        {
            ES = es;
            transform = es.transform;
            Movement = movement;
            this.animator = animator;
        }
    }
}