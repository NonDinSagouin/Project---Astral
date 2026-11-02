using UnityEngine;

using Helper.Debug;

namespace Core.Binder
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasCameraBinder : MonoBehaviour
    {
        [SerializeField] private bool debugMode;

        private void Start()
        {
            Canvas canvas = GetComponent<Canvas>();
            Camera mainCamera = Camera.main;

            if (mainCamera == null)
            {
                throw new System.Exception("CanvasCameraBinder : aucune caméra principale trouvée.");
            }

            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = mainCamera;

            DebugMessage.Log(debugMode, "CanvasCameraBinder : caméra principale assignée au canvas.", this);
        }
    }
}