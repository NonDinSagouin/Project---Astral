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

        public Vector3 WorldMousePosition => worldMousePosition;
        public bool HasWorldMousePosition { get; private set; }

        void Update()
        {
            HasWorldMousePosition = false;
            if (Mouse.current == null) return;

            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;

            Vector3 mousePosition = Mouse.current.position.ReadValue();
            worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
            worldMousePosition.z = 0; // Ensure the world mouse position is on the XY plane

            HasWorldMousePosition = true;
            DebugMessage.Log(debugMode, $"Mouse Position: {mousePosition}, World Mouse Position: {worldMousePosition}");
            Cursor.visible = showCursor;
        }
    }
}