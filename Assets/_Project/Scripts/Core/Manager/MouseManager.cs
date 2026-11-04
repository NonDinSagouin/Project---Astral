using UnityEngine;
using UnityEngine.InputSystem;

using Helper.Debug;

namespace Core.Manager
{
    public class MouseManager : Singleton<MouseManager>
    {
        [SerializeField] private bool debugMode;
        [SerializeField] private bool showCursor;

        public Vector3 WorldMousePosition { get; private set; }
        public bool HasWorldMousePosition { get; private set; }
        
        public bool WasLeftMousePressedThisFrame { get; private set; }
        public bool WasLeftMouseReleasedThisFrame { get; private set; }
        public bool IsLeftMousePressed { get; private set; }

        public bool WasRightMousePressedThisFrame { get; private set; }
        public bool WasRightMouseReleasedThisFrame { get; private set; }
        public bool IsRightMousePressed { get; private set; }

        void Start()
        {
            if (Mouse.current == null)
            {
                throw new System.Exception("No mouse detected !");
            }
        }

        void Update()
        {
            Mouse mouse = Mouse.current;

            UpdateButtons(mouse);
            UpdatePosition(mouse);
        }

        private void UpdateButtons(Mouse mouse)
        {
            if (mouse == null)
            {
                WasLeftMousePressedThisFrame = false;
                WasLeftMouseReleasedThisFrame = false;
                IsLeftMousePressed = false;
                
                WasRightMousePressedThisFrame = false;
                WasRightMouseReleasedThisFrame = false;
                IsRightMousePressed = false;
                return;
            }

            /// Update the left mouse button states based on the current mouse input
            WasLeftMousePressedThisFrame = mouse.leftButton.wasPressedThisFrame;
            WasLeftMouseReleasedThisFrame = mouse.leftButton.wasReleasedThisFrame;
            IsLeftMousePressed = mouse.leftButton.isPressed;

            /// Update the right mouse button states based on the current mouse input
            WasRightMousePressedThisFrame = mouse.rightButton.wasPressedThisFrame;
            WasRightMouseReleasedThisFrame = mouse.rightButton.wasReleasedThisFrame;
            IsRightMousePressed = mouse.rightButton.isPressed;
        }

        private void UpdatePosition(Mouse mouse)
        {
            HasWorldMousePosition = false;
            if (mouse == null) return;

            Camera mainCamera = Camera.main;
            if (mainCamera == null) return;

            Vector3 mousePosition = mouse.position.ReadValue();

            /// Convert the screen mouse position to world position on the XY plane
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0;
            WorldMousePosition = worldPosition;

            /// Update the flag indicating that the world mouse position is valid
            HasWorldMousePosition = true;
            DebugMessage.Log(debugMode, $"Mouse Position: {mousePosition}, World Mouse Position: {WorldMousePosition}");
            Cursor.visible = showCursor;
        }
    }
}