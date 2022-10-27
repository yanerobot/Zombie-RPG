using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using System.Threading.Tasks;

namespace KK
{
    public class GameManager : MonoBehaviour
    {
        public const string Tag = "GameManager";
        int currentSceneBuildIndex;

        void Awake()
        {
            currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        }
        public void LoadScene(int buildIndex)
        {
            if (buildIndex >= SceneManager.sceneCountInBuildSettings || buildIndex < 0)
            {
                Debug.LogError($"Invalid scene was provided: {buildIndex}");
                return;
            }

            SceneManager.LoadSceneAsync(buildIndex);
            currentSceneBuildIndex = buildIndex;
        }

        public void LoadNextScene()
        {
            LoadScene(currentSceneBuildIndex + 1);
        }

        public void RestartScene()
        {
            LoadScene(currentSceneBuildIndex);
        }
    } 
}
