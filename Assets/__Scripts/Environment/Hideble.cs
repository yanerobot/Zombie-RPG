using UnityEngine;
using DG.Tweening;

namespace KK
{
    public class Hideble : MonoBehaviour
    {
        [SerializeField] MeshRenderer visible, invisible;

        public void Hide()
        {
            visible.gameObject.SetActive(false);
            invisible.gameObject.SetActive(true); 
            invisible.material.DOFade(0.3f, 0.5f);
        }

        public void Show()
        {
            invisible.material.DOFade(1, 0.5f).OnComplete(() =>
            {
                visible.gameObject.SetActive(true);
                invisible.gameObject.SetActive(false);
            });
        }
    }
}
