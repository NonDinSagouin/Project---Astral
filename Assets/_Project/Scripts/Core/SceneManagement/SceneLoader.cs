using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Helper;

namespace Core.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private bool isLoadingScene;
        [SerializeField] private bool debugMode;

        [SerializeField] private List<string> scenesAllowedToLoadMultipleTimes = new List<string>();
        
        /// <summary>
        /// Charge la scène spécifiée de manière asynchrone.
        /// </summary>
        /// <param name="sceneName">Le nom de la scène à charger.</param>
        public bool LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                DebugMessage.Log(debugMode, "Cannot load a scene because its name is empty.", this);
                return false;
            }

            if (isLoadingScene)
            {
                DebugMessage.LogWarning(debugMode, $"Cannot load scene '{sceneName}' while another scene is loading.", this);
                return false;
            }

            bool canLoadMultipleTimes = loadSceneMode != LoadSceneMode.Additive || !scenesAllowedToLoadMultipleTimes.Contains(sceneName);
            if (SceneManager.GetSceneByName(sceneName).isLoaded && canLoadMultipleTimes)
            {
                DebugMessage.LogWarning(debugMode, $"Scene '{sceneName}' is already loaded and cannot be loaded again.", this);
                return false;
            }
    
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                DebugMessage.LogError(debugMode, $"Scene '{sceneName}' is not included in the build settings.", this);
                return false;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            if (operation == null)
            {
                DebugMessage.LogError(debugMode, $"Failed to start loading scene '{sceneName}'.", this);
                return false;
            }

            isLoadingScene = true;
            operation.completed += _ => isLoadingScene = false;
            return true;
        }

        /// <summary>
        /// Décharge la scène spécifiée de manière asynchrone.
        /// </summary>
        /// <param name="sceneName">Le nom de la scène à décharger.</param>
        /// <returns>Retourne true si le déchargement a été initié avec succès, sinon false.</returns>
        public bool UnloadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                DebugMessage.Log(debugMode, "Cannot unload a scene because its name is empty.", this);
                return false;
            }

            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                DebugMessage.LogWarning(debugMode, $"Scene '{sceneName}' is not loaded.", this);
                return false;
            }

            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);
            if (operation == null)
            {
                DebugMessage.LogError(debugMode, $"Failed to start unloading scene '{sceneName}'.", this);
                return false;
            }

            isLoadingScene = true;
            operation.completed += _ => isLoadingScene = false;
            return true;
        }
    }
}