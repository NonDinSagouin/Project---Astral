using UnityEngine;
using Data;

using Core;
using Core.Manager;
using Core.Event;
using Core.Localization;

namespace Testing
{
    public class TestingClass : MonoBehaviour
    {
        public void loadScene(string sceneName)
        {
            Debug.Log("Loading scene: " + sceneName);
            SceneLoaderManager.Instance.LoadScene(sceneName);
        }
        
        public void unloadScene(string sceneName)
        {
            Debug.Log("Unloading scene: " + sceneName);
            SceneLoaderManager.Instance.UnloadScene(sceneName);
        }

        public void switchToFrench()
        {
            Debug.Log("Switching to French...");
            LocalizationManager.Instance.SwitchLanguage(Language.Fr);
        }

        public void switchToEnglish()
        {
            Debug.Log("Switching to English...");
            LocalizationManager.Instance.SwitchLanguage(Language.En);
        }
    }
}
