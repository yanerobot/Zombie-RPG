using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace KK
{
    public class EnemyAIManager : MonoBehaviour
    {
        public Health target;
        public float radiusAroundTarget;

        Dictionary<EnemyAI, Vector3> offsets;

        public MeleeWeaponController targetWeaponController;

        void Awake()
        {
            offsets = new Dictionary<EnemyAI, Vector3>();
            int index = 0;
            foreach (Transform enemyTransform in transform)
            {
                var enemy = enemyTransform.GetComponent<EnemyAI>();

                offsets.Add(enemy, CalculateCircleOffset(index));

                enemy.aiManager = this;
                index++;
            }
        }

        IEnumerator Start()
        {
            var updateMode = new WaitForSeconds(0.5f);
            while (target == null)
            {
                FindTarget();
                yield return updateMode;
            }

        }
        public void FindTarget()
        {
            var obj = GameObject.FindWithTag("Player");

            if (obj == null)
                return;

            obj.TryGetComponent(out Health health);
            if (!health.isDead)
            {
                target = health;
                targetWeaponController = target.GetComponent<MeleeWeaponController>();
            }
        }

        public bool IsTargetAvailible()
        {
            if (target == null)
                return false;

            if (target.isDead)
                return false;

            return true;
        }

        public Vector3 GetBestPositionAroundTarget(EnemyAI enemy)
        {
            offsets.TryGetValue(enemy, out var offset);

            return target.transform.position + offset;
        }

        Vector3 CalculateCircleOffset(int id)
        {
            Vector3 circleOffset = new Vector3(
                    x: radiusAroundTarget * Mathf.Cos(2 * Mathf.PI * id / transform.childCount),
                    y: 0,
                    z: radiusAroundTarget * Mathf.Sin(2 * Mathf.PI * id / transform.childCount)
                );

            return circleOffset;
        }
    }
}
