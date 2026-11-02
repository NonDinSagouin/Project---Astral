using UnityEngine;

namespace Gameplay.Combat.Grid
{
    public class Cell
    {
        private bool isHovered;

        private Vector2Int coordinates;
        private Vector3 worldPosition;

        public Vector2Int Coordinates => coordinates;
        public Vector3 WorldPosition => worldPosition;
        public bool IsHovered => isHovered;

        public Cell(Vector2Int coordinates, Vector3 worldPosition)
        {
            this.coordinates = coordinates;
            this.worldPosition = worldPosition;
            isHovered = false;
        }

        public void SetHovered(bool hovered)
        {
            isHovered = hovered;
        }
    }
}