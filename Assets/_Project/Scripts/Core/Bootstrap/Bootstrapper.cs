using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

using Helper.Debug;
using Core.Manager;

namespace Core.Bootstrap
{
    /// <summary>
    /// Classe responsable de l'initialisation des services de bootstrap et du chargement de la scène initiale.
    /// </summary>
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private SceneLoaderManager sceneLoader;
        [SerializeField] private string[] startSceneName;

        [SerializeField] private bool debugMode;

        private readonly HashSet<string> scenesToLoad = new HashSet<string>();

        void Awake()
        {
            if (sceneLoader == null)
            {
                throw new ArgumentException("SceneLoader is not assigned on the Bootstrapper.", nameof(sceneLoader));
            }

            if (startSceneName == null || startSceneName.Length == 0 || string.IsNullOrWhiteSpace(startSceneName[0]))
            {
                throw new ArgumentException("Initial scene name cannot be empty.", nameof(startSceneName));
            }
        }

        void Start()
        {
            DebugMessage.Log(debugMode, "Bootstrapper Start: Loading initial scene.", this);

            if (startSceneName == null || startSceneName.Length == 0 || string.IsNullOrWhiteSpace(startSceneName[0]))
            {
                throw new ArgumentException("Initial scene name cannot be empty.", nameof(startSceneName));
            }

            if (sceneLoader == null)
            {
                throw new ArgumentException("SceneLoader is not assigned on the Bootstrapper.", nameof(sceneLoader));
            }

            scenesToLoad.Clear();
            List<string> scenesToRequest = new List<string>();

            foreach (string sceneName in startSceneName)
            {
                if (string.IsNullOrWhiteSpace(sceneName))
                {
                    throw new ArgumentException("Initial scene name cannot be empty.", nameof(startSceneName));
                }

                if (sceneLoader.IsAlreadyLoaded(sceneName))
                {
                    DebugMessage.LogWarning(debugMode, $"Scene '{sceneName}' is already loaded.", this);
                    continue;
                }

                if (scenesToLoad.Add(sceneName))
                {
                    scenesToRequest.Add(sceneName);
                }
            }

            if (scenesToLoad.Count == 0)
            {
                UnloadBootstrapScene();
                return;
            }

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            foreach (string sceneName in scenesToRequest)
            {
                DebugMessage.Log(debugMode, $"Loading initial scene: {sceneName}", this);
                if (!sceneLoader.LoadScene(sceneName, LoadSceneMode.Additive))
                {
                    UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
                    throw new InvalidOperationException($"Failed to load initial scene '{sceneName}'.");
                }
            }
        }

        void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (!scenesToLoad.Remove(scene.name))
            {
                return;
            }

            if (scenesToLoad.Count > 0)
            {
                return;
            }

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
            DebugMessage.Log(debugMode, "All initial scenes loaded. Unloading the Bootstrap scene.", this);
            UnloadBootstrapScene();
        }

        private void UnloadBootstrapScene()
        {
            string bootstrapSceneName = gameObject.scene.name;
            if (startSceneName != null && Array.IndexOf(startSceneName, bootstrapSceneName) >= 0)
            {
                DebugMessage.LogError(debugMode, "The Bootstrap scene cannot be unloaded because it is listed as a start scene.", this);
                return;
            }

            sceneLoader.UnloadScene(bootstrapSceneName);
        }
    }
}
