using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] private string initialSceneName = "MainMenu";
        
        [SerializeField] private MonoBehaviour[] services;
        [SerializeField] private SceneLoader sceneLoader;

        void Awake()
        {
            InitializeServices();
        }

        void Start()
        {
            if (instance == this && !string.IsNullOrWhiteSpace(initialSceneName))
            {
                sceneLoader.LoadScene(initialSceneName);
            }
        }

        /// <summary>
        /// Initialise tous les services de bootstrap référencés dans le tableau des services.
        /// </summary>
        private void InitializeServices()
        {
            if (services == null)
            {
                return;
            }

            foreach (MonoBehaviour service in services)
            {
                if (service == null)
                {
                    Debug.LogError("A service reference is missing on the Bootstrapper.", this);
                    continue;
                }

                if (service is not IBootstrapService bootstrapService)
                {
                    Debug.LogError($"{service.name} does not implement {nameof(IBootstrapService)}.", service);
                    continue;
                }

                bootstrapService.Initialize();
            }
        }
    }
}
