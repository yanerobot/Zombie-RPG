using UnityEngine;
using System.Collections;

namespace KK.Items
{
    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] RectTransform eButtonTip;
        [SerializeField] Vector3 offset;

        EquipmentSystem es;
        Camera cam;

        ItemObject bestItem;

        IEnumerator Start()
        {
            GameObject player = null;

            while (player == null)
            {
                player = GameObject.FindWithTag(PlayerMain.Tag);

                yield return new WaitForSeconds(0.5f);
            }

            player.TryGetComponent(out es);
            cam = Camera.main;
        }

        void Update()
        {
            if (es == null)
                return;

            if (es.isEquipCooldown)
            {
                eButtonTip.gameObject.SetActive(false);
                return;
            }

            bestItem = es.GetBestItem();
            if (bestItem == null)
            {
                eButtonTip.gameObject.SetActive(false);
                return;
            }

            eButtonTip.gameObject.SetActive(true);

            var wp = cam.WorldToScreenPoint(bestItem.transform.position + offset);
            eButtonTip.position = wp;
        }
    }
}
