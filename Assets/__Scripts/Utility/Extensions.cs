using System;
using System.Collections;
using UnityEngine;

namespace KK.Utility
{
    public static class Extensions
    {
        #region Vector3

        public static Vector3 Where(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            if (x == null)
                x = vector.x;
            if (y == null)
                y = vector.y;
            if (z == null)
                z = vector.z;

            return new Vector3((float)x, (float)y, (float)z);
        }

        public static Vector3 AddTo(this Vector3 vector, float x = 0, float y = 0, float z = 0)
        {
            vector.x += x;
            vector.y += y;
            vector.z += z;
            return vector;
        }
        public static Vector3 Multiply(this Vector3 vector, float x = 1, float y = 1, float z = 1)
        {
            vector.x *= x;
            vector.y *= y;
            vector.z *= z;
            return vector;
        }
        public static Vector3 Direction(this Vector3 currentPos, Vector3 targetPos)
        {
            return (targetPos - currentPos).normalized;
        }
        #endregion

        #region Vector2
        public static Vector2 WhereX(this Vector2 vector, float x)
        {
            vector.x = x;
            return vector;
        }
        public static Vector2 WhereY(this Vector2 vector, float y)
        {
            vector.y = y;
            return vector;
        }
        public static Vector2 AddTo(this Vector2 vector, float x = 0, float y = 0)
        {
            vector.x += x;
            vector.y += y;
            return vector;
        }
        public static Vector2 Multiply(this Vector2 vector, float x = 1, float y = 1)
        {
            vector.x *= x;
            vector.y *= y;
            return vector;
        }
        public static Vector2 Direction(this Vector2 currentPos, Vector2 targetPos)
        {
            return (targetPos - currentPos).normalized;
        }
        #endregion

        #region Transform

        public static void ForEachParent(this Transform parent, Action<Transform> action)
        {
            action(parent);

            foreach (Transform child in parent)
            {
                ForEachParent(child, action);
            }
        }

        public static void ForEachChild(this Transform parent, Action<Transform> action)
        {
            foreach (Transform child in parent)
            {
                action(child);
                ForEachChild(child, action);
            }
        }

        public static void RotateTowards(this Transform transform, Vector3 direction, bool smooth = false, float turnSpeed = 0)
        {
            if (direction == Vector3.zero)
                return;

            direction = direction.normalized;

            var rotation = Quaternion.LookRotation(direction, Vector3.up);

            transform.rotation = smooth ? Quaternion.RotateTowards(transform.rotation, rotation, turnSpeed) : rotation;
        }

        #endregion

        #region Coroutine
        /// <summary> Calls function with delay (seconds). Coroutine </summary>
        public static Coroutine Co_DelayedExecute(this MonoBehaviour caller, Action action, float delay, bool scaledTime = true) => caller.StartCoroutine(Utils.Co_DelayedExecute(action, delay, scaledTime));

        /// <summary> Calls function with delay (Update frames). Coroutine </summary>
        public static Coroutine Co_DelayedExecute(this MonoBehaviour caller, Action action, int framesDelay) => caller.StartCoroutine(Utils.Co_DelayedExecute(action, framesDelay));

        /// <summary> Calls function when "predicate" function evaluates to true. Coroutine </summary>
        public static Coroutine Co_DelayedExecute(this MonoBehaviour caller, Action action, Func<bool> predicate, float minTime = 0) => caller.StartCoroutine(Utils.Co_DelayedExecute(action, predicate, minTime));
        #endregion

        #region LayerMask

        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }

        #endregion

        #region

        public static IEnumerator SmoothAnimationStateBlendCoroutine(this Animator animator, string parameterName, float targetValue, float interpolationSpeed = 1, float clampTargetValue = 1)
        {
            var startValue = animator.GetFloat(parameterName);

            var currentValue = startValue;

            var interpolation = 0f;

            targetValue = Mathf.Clamp(targetValue, 0, targetValue / clampTargetValue);

            while (currentValue != targetValue)
            {
                currentValue = Mathf.SmoothStep(startValue, targetValue, interpolation);

                animator.SetFloat(parameterName, currentValue);

                interpolation += Time.deltaTime * interpolationSpeed;
                yield return null;
            }
        }

        #endregion
    }

}