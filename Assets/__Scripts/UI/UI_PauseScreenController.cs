using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace KK
{
    public class UI_PauseScreenController : MonoBehaviour
    {
        [SerializeField] Button restartButton;
        [SerializeField] Button optionsButton;
        [SerializeField] Button exitButton;
        GameManager gameManager;
        
        IEnumerator Start()
        {
            GameObject gmObj = null;
            while (gmObj == null)
            {
                gmObj = GameObject.FindWithTag(GameManager.Tag);
                yield return new WaitForSeconds(0.5f);
            }
        }

        void Init(GameObject gmObj)
        {
            gameManager = gmObj.GetComponent<GameManager>();

            restartButton.onClick.AddListener(gameManager.RestartScene);
            //add more features
        }

        void OnDestroy()
        {
            restartButton.onClick.RemoveAllListeners();
        }
    }
}
