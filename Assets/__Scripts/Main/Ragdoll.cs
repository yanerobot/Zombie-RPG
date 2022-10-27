using UnityEngine;
using KK.Utility;
using System.Collections.Generic;
using System.Collections;

namespace KK
{
    public class Ragdoll : MonoBehaviour
    {
        [SerializeField] Behaviour[] componentsToDisable;
        [SerializeField] Collider mainCollider;
        [SerializeField] Transform rootBone;

        public const string LAYER = "Ragdoll";

        List<Collider> allColliders = new List<Collider>();
        void Awake()
        {
            FindRagdollParts();
            RagdollSetActive(false);
        }

        void FindRagdollParts()
        {
            allColliders.Clear();
            rootBone.ForEachParent((parent) =>
            {
                if (parent.gameObject.layer == LayerMask.NameToLayer(LAYER) && parent.TryGetComponent(out Collider coll))
                {
                    allColliders.Add(coll);
                }
            });
        }

        public void RagdollSetActive(bool enabled)
        {
            if (mainCollider != null)
                mainCollider.enabled = !enabled;
            
            foreach (var component in componentsToDisable)
            {
                component.enabled = !enabled;
            }

            foreach (var coll in allColliders)
            {
                try
                {
                    coll.attachedRigidbody.isKinematic = !enabled;
                }
                catch
                {
                    coll.TryGetComponent(out Rigidbody rb);
                    rb.isKinematic = !enabled;
                }
                finally
                {
                    coll.enabled = enabled;
                }
            }
        }
    }
}
