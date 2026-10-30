using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Helper;

namespace Core.Managers
{
    public class SceneLoaderManager : MonoBehaviour
    {
        [SerializeField] private bool debugMode;
        [SerializeField] private List<string> scenesLoaded = new List<string>();
        [SerializeField] private List<string> scenesAllowedToLoadMultipleTimes = new List<string>();
        [SerializeField] private List<string> scenesNotAllowedToUnload = new List<string>();

        /// <summary>
        /// Vérifie si la scène spécifiée est déjà chargée.
        /// </summary>
        /// <param name="sceneName">Le nom de la scène à vérifier.</param>
        /// <returns>Retourne true si la scène est déjà chargée, sinon false.</returns>
        public bool IsAlreadyLoaded(string sceneName)
        {
            if (scenesAllowedToLoadMultipleTimes.Contains(sceneName)) return false;
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }

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

            if (!scenesLoaded.Contains(sceneName)) scenesLoaded.Add(sceneName);

            return true;
        }

        /// <summary>
        /// Décharge la scène spécifiée de manière asynchrone.
        /// </summary>
        /// <param name="sceneName">Le nom de la scène à décharger.</param>
        /// <returns>Retourne true si le déchargement a été initié avec succès, sinon false.</returns>
        public bool UnloadScene(string sceneName)
        {
            if (scenesNotAllowedToUnload.Contains(sceneName))
            {
                DebugMessage.LogWarning(debugMode, $"Scene '{sceneName}' is not allowed to be unloaded.", this);
                return false;
            }

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

            if (scenesLoaded.Contains(sceneName)) scenesLoaded.Remove(sceneName);

            return true;
        }
    }
}