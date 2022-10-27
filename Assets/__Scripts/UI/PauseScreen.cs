using UnityEngine;

namespace KK
{
    [RequireComponent(typeof(CanvasHandler))]
    public class PauseScreen : MonoBehaviour
    {
        CanvasHandler canvasHandler;

        InputMaster input;

        void Awake()
        {
            canvasHandler = GetComponent<CanvasHandler>();
            input = new InputMaster();
        }

        void Start()
        {
            input.UI.Enable();
            input.UI.Pause.performed += _ =>
            {
                canvasHandler.ToggleScreen("PauseScreen");
                Time.timeScale = Time.timeScale == 0 ? 1 : 0;
            };
        }

        void OnDestroy()
        {
            input.Dispose();
            Time.timeScale = 1;
        }
    }
}
