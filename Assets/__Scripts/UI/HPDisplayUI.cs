using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

namespace KK
{
    public class HPDisplayUI : MonoBehaviour
    {
        [SerializeField] Image hpFill;
        [SerializeField] Image hpAnimation;
        [SerializeField] float animationDuration;
        Health health;

        IEnumerator Start()
        {
            GameObject player = null;

            while (player == null)
            {
                player = GameObject.FindWithTag("Player");
                yield return new WaitForSeconds(0.5f);
            }

            Init(player);
        }

        void OnDestroy()
        {
            health?._OnDamage.RemoveListener(DisplayHealth);
            health?._OnRestore.RemoveListener(DisplayHealth);
            health?._OnDie.RemoveListener(DisplayZeroHealth);
        }

        void Init(GameObject player)
        {
            health = player.GetComponent<Health>();

            hpFill.fillAmount = health.currentHealth / health.maxHealth;
            hpAnimation.fillAmount = hpFill.fillAmount;

            health._OnDamage.AddListener(DisplayHealth);
            health._OnRestore.AddListener(DisplayHealth);
            health._OnDie.AddListener(DisplayZeroHealth);
        }

        void DisplayZeroHealth()
        {
            DOVirtual.Float(hpFill.fillAmount, 0, animationDuration, x => hpAnimation.fillAmount = x);
            hpFill.fillAmount = 0;
        }

        void DisplayHealth(int newHealth)
        {
            var newFillValue = (float)newHealth / health.maxHealth;
            DOVirtual.Float(hpFill.fillAmount, newFillValue, animationDuration, x => hpAnimation.fillAmount = x);
            hpFill.fillAmount = newFillValue;
        }
    }
}
