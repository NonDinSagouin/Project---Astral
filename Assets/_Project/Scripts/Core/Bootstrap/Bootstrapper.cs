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
        [Tooltip("Scenes loaded first. The start scenes are loaded once all of them are loaded.")]
        [SerializeField] private string[] prioritySceneName;
        [SerializeField] private string[] startSceneName;
        [SerializeField] private bool debugMode;

        private readonly HashSet<string> scenesToLoad = new HashSet<string>();
        private Action onScenesLoaded;

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
            DebugMessage.Log(debugMode, "Bootstrapper Start: Loading priority scenes then initial scenes.", this);

            if (startSceneName == null || startSceneName.Length == 0 || string.IsNullOrWhiteSpace(startSceneName[0]))
            {
                throw new ArgumentException("Initial scene name cannot be empty.", nameof(startSceneName));
            }

            if (sceneLoader == null)
            {
                throw new ArgumentException("SceneLoader is not assigned on the Bootstrapper.", nameof(sceneLoader));
            }

            LoadScenes(prioritySceneName, nameof(prioritySceneName), LoadStartScenes);
        }

        private void LoadStartScenes()
        {
            LoadScenes(startSceneName, nameof(startSceneName), UnloadBootstrapScene);
        }

        /// <summary>
        /// Charge un groupe de scènes en parallèle et appelle onCompleted quand toutes sont chargées.
        /// </summary>
        private void LoadScenes(string[] sceneNames, string fieldName, Action onCompleted)
        {
            scenesToLoad.Clear();
            List<string> scenesToRequest = new List<string>();

            foreach (string sceneName in sceneNames ?? Array.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(sceneName))
                {
                    throw new ArgumentException("Scene name cannot be empty.", fieldName);
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
                onCompleted();
                return;
            }

            onScenesLoaded = onCompleted;
            SceneManager.sceneLoaded += OnSceneLoaded;
            foreach (string sceneName in scenesToRequest)
            {
                DebugMessage.Log(debugMode, $"Loading scene from {fieldName}: {sceneName}", this);
                if (!sceneLoader.LoadScene(sceneName, LoadSceneMode.Additive))
                {
                    SceneManager.sceneLoaded -= OnSceneLoaded;
                    onScenesLoaded = null;
                    throw new InvalidOperationException($"Failed to load scene '{sceneName}' from {fieldName}.");
                }
            }
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
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

            SceneManager.sceneLoaded -= OnSceneLoaded;
            Action onCompleted = onScenesLoaded;
            onScenesLoaded = null;
            onCompleted?.Invoke();
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
