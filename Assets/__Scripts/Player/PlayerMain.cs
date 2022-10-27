using UnityEngine;
using KK.Utility;

namespace KK
{
    public class PlayerMain : MonoBehaviour
    {
        public const string Tag = "Player";
        public const string Layer = "Player";

        [SerializeField] Animator animator;
        [SerializeField] Health health;

        void Awake()
        {
            health._OnDie.AddListener(ShowGameOverScreenWithDelay);
        }

        void ShowGameOverScreenWithDelay() => this.Co_DelayedExecute(ShowGameOverScreen, 2f);

        void ShowGameOverScreen() => FindObjectOfType<CanvasHandler>()?.ToggleGameOverScreen(true);


        void OnDestroy()
        {
            health._OnDie.RemoveAllListeners();
        }

        public void OnHit()
        {
            animator.SetTrigger("GotHit");
            this.Co_DelayedExecute(() => animator.ResetTrigger("GotHit"), 5);
        }
    }
}
