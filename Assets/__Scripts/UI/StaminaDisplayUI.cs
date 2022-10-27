using UnityEngine;
using UnityEngine.UI;
using KK.Player;
using System.Collections;
using DG.Tweening;

namespace KK
{
    public class StaminaDisplayUI : MonoBehaviour
    {
        [SerializeField] Image staminaFill;
        [SerializeField] Image staminaAnimation;
        [SerializeField] float animationDuration;
        Stamina stamina;

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
            if (stamina != null)
            {
                stamina.OnStaminaChange.RemoveListener(DisplayStamina);
                stamina.OnStaminaEmpty.RemoveListener(DisplayEmptyStamina);
                stamina.OnStaminaFull.RemoveListener(DisplayFullStamina);

                stamina.OnStaminaInstantChange -= DisplayStaminaAnimation;
            }

            DOTween.Clear(); 
        }

        void Init(GameObject player)
        {
            stamina = player.GetComponent<Stamina>();

            staminaFill.fillAmount = stamina.currentStamina / stamina.maxStamina;
            staminaAnimation.fillAmount = stamina.currentStamina / stamina.maxStamina;

            stamina.OnStaminaChange.AddListener(DisplayStamina);
            stamina.OnStaminaEmpty.AddListener(DisplayEmptyStamina);
            stamina.OnStaminaFull.AddListener(DisplayFullStamina);

            stamina.OnStaminaInstantChange += DisplayStaminaAnimation;
        }

        void DisplayStaminaAnimation(float newValue)
        {
            DOVirtual.Float(staminaAnimation.fillAmount, stamina.currentStamina / stamina.maxStamina, animationDuration, x => staminaAnimation.fillAmount = x);
        }

        void DisplayEmptyStamina()
        {
            staminaFill.fillAmount = 0;
        }

        void DisplayFullStamina()
        {
            staminaAnimation.fillAmount = 1;
        }

        void DisplayStamina()
        {
            float newFillValue = stamina.currentStamina / stamina.maxStamina;

            staminaFill.fillAmount = newFillValue;
        }
    }
}
