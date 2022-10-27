using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using System;

namespace KK
{
    public class RigController : MonoBehaviour
    {
        [SerializeField, Min(1)] float interpolationSpeed;
        [Header("Right hand")]
        [SerializeField] TwoBoneIKConstraint rightHandConstraint;
        [SerializeField] Transform rightHandTarget;
        [SerializeField] Transform onRunTargetTransform;
        [SerializeField, Range(0, 1)] float enabledRightHandIKWeight, disabledRightHandIKWeight;
        float newWeightRightHand, previousWeightRightHand;
        float interpolationRightHand;
        [Header("Left hand")]
        [SerializeField] TwoBoneIKConstraint leftHandConstraint;
        [SerializeField, Range(0, 1)] float enabledLeftHandIKWeight, disabledLeftHandIKWeight;
        float newWeightLeftHand, previousWeightLeftHand;
        float interpolationleftHand;

        Vector3 initialRightHandPos;

        void Awake()
        {
            //initialRightHandPos = rightHandTarget.position;
            SetRightHandConstraintWeight(false);
            SetLeftHandConstraintWeght(false);
        }

        public void SetRightHandConstraintWeight(bool enabled)
        {
            interpolationRightHand = 0;
            previousWeightRightHand = newWeightRightHand;
            newWeightRightHand = enabled ? enabledRightHandIKWeight : disabledRightHandIKWeight;
        }

        public void SetRightHandPositionTo()
        {
            rightHandTarget.DOMove(onRunTargetTransform.position, 0.3f);
        }

        public void SetInitialRightHandPosition()
        {
            rightHandTarget.DOMove(initialRightHandPos, 0.3f);
        }

        public void SetLeftHandConstraintWeght(bool enabled)
        {
            interpolationleftHand = 0;
            previousWeightLeftHand = newWeightLeftHand;
            newWeightLeftHand = enabled ? enabledLeftHandIKWeight : disabledLeftHandIKWeight;
        }

        void Update()
        {
            rightHandConstraint.weight = Mathf.SmoothStep(previousWeightRightHand, newWeightRightHand, interpolationRightHand);

            leftHandConstraint.weight = Mathf.SmoothStep(previousWeightLeftHand, newWeightLeftHand, interpolationleftHand);

            if (interpolationRightHand <= 1)
                interpolationRightHand += Time.deltaTime * interpolationSpeed;
            if (interpolationleftHand <= 1)
                interpolationleftHand += Time.deltaTime * interpolationSpeed;
        }
    }
}
