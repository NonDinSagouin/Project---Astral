using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private bool isLoadingScene;
        
        /// <summary>
        /// Charge la scène spécifiée de manière asynchrone.
        /// </summary>
        /// <param name="sceneName">Le nom de la scène à charger.</param>
        public bool LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("Cannot load a scene because its name is empty.", this);
                return false;
            }

            if (isLoadingScene)
            {
                Debug.LogWarning($"Cannot load scene '{sceneName}' while another scene is loading.", this);
                return false;
            }

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"Scene '{sceneName}' is not included in the build settings.", this);
                return false;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            if (operation == null)
            {
                Debug.LogError($"Failed to start loading scene '{sceneName}'.", this);
                return false;
            }

            isLoadingScene = true;
            operation.completed += _ => isLoadingScene = false;
            return true;
        }
    }
}