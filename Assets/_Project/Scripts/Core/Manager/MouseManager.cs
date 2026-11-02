using UnityEngine;
using UnityEngine.InputSystem;

using Helper.Debug;

namespace Core.Manager
{
    public class MouseManager : Singleton<MouseManager>
    {
        [SerializeField] private bool debugMode;
        [SerializeField] private bool showCursor;

        [SerializeField] private Vector3 worldMousePosition;

        void Update()
        {
            if (Mouse.current == null) return;
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = 0; // Ensure the world mouse position is on the XY plane

            DebugMessage.Log(debugMode, $"Mouse Position: {mousePosition}, World Mouse Position: {worldMousePosition}");
            Cursor.visible = showCursor;
        }
    }
}