using UnityEngine;
using UnityEngine.EventSystems;
using KK.Utility;

namespace KK
{
    public class PreviewModelRotator : MonoBehaviour, IDragHandler
    {
        [SerializeField] Transform previewModel;

        Vector3 initialRotation;

        void Awake()
        {
            initialRotation = previewModel.eulerAngles;
        }
        public void OnDrag(PointerEventData eventData)
        {
            var rotationY = -eventData.delta.x;

            previewModel.eulerAngles = previewModel.eulerAngles.AddTo(y: rotationY);
        }

        void OnDisable()
        {
            previewModel.eulerAngles = initialRotation;
        }
    }
}
