using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Helper;
using Core.SceneManagement;

namespace Core.Bootstrap
{
    /// <summary>
    /// Interface définissant un service de bootstrap qui doit être initialisé par le Bootstrapper.
    /// </summary>
    public interface IBootstrapService
    {
        void Initialize();
    }

    /// <summary>
    /// Classe responsable de l'initialisation des services de bootstrap et du chargement de la scène initiale.
    /// </summary>
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private string initialSceneName = "MainMenu";
        [SerializeField] private MonoBehaviour[] services;

        [SerializeField] private bool debugMode;

        void Awake()
        {
            DebugMessage.Log(debugMode, "Bootstrapper Awake: Initializing services.", this);
            InitializeServices();
        }

        void Start()
        {
            DebugMessage.Log(debugMode, "Bootstrapper Start: Loading initial scene.", this);

            if (string.IsNullOrWhiteSpace(initialSceneName))
            {
                throw new ArgumentException("Initial scene name cannot be empty.", nameof(initialSceneName));
            }

            if (sceneLoader == null)
            {
                throw new ArgumentException("SceneLoader is not assigned on the Bootstrapper.", nameof(sceneLoader));
            }

            DebugMessage.Log(debugMode, $"Loading initial scene: {initialSceneName}", this);
            SceneManager.sceneLoaded += OnSceneLoaded;

            if (!sceneLoader.LoadScene(initialSceneName, LoadSceneMode.Additive))
            {
                throw new InvalidOperationException($"Failed to load initial scene '{initialSceneName}'.");
            }
        }

        void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name != initialSceneName)
            {
                throw new InvalidOperationException($"Loaded scene '{scene.name}' does not match the initial scene '{initialSceneName}'.");
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
            DebugMessage.Log(debugMode, "Initial scene loaded. Destroying Bootstrapper.", this);

            Destroy(this);
        }

        /// <summary>
        /// Initialise tous les services de bootstrap référencés dans le tableau des services.
        /// </summary>
        private void InitializeServices()
        {
            if (services == null)
            {
                DebugMessage.LogError(debugMode, "No services assigned to the Bootstrapper.", this);
                return;
            }

            foreach (MonoBehaviour service in services)
            {
                if (service == null)
                {
                    DebugMessage.LogError(debugMode, "A service reference is missing on the Bootstrapper.", this);
                    continue;
                }

                if (service is not IBootstrapService bootstrapService)
                {
                    DebugMessage.LogError(debugMode, $"{service.name} does not implement {nameof(IBootstrapService)}.", service);
                    continue;
                }

                DebugMessage.Log(debugMode, $"Initializing service: {service.name}", service);
                bootstrapService.Initialize();
            }
        }
    }
}
