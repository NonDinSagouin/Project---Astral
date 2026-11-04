using UnityEngine;

namespace Gameplay.Combat.Grid
{
    public class Cell
    {
        private bool isHovered;
        private bool isSelected;
        private bool isOccupied;
        private bool isObstacle;
        
        private CellOccupant occupant;

        private Vector2Int coordinates;
        private Vector3 worldPosition;

        public Vector2Int Coordinates => coordinates;
        public Vector3 WorldPosition => worldPosition;
        public bool IsHovered => isHovered;
        public bool IsSelected => isSelected;
        public bool IsOccupied => isOccupied;
        public bool IsObstacle => isObstacle;
        public CellOccupant Occupant => occupant;

        public Cell(Vector2Int coordinates, Vector3 worldPosition)
        {
            this.coordinates = coordinates;
            this.worldPosition = worldPosition;
            isHovered = false;
            isSelected = false;
            isOccupied = false;
            isObstacle = false;
        }

        /// <summary>
        /// Définit l'état de survol de la cellule.
        /// </summary>
        /// <param name="hovered">Indique si la cellule est survolée.</param>
        public void SetIsHovered(bool hovered)
        {
            isHovered = hovered;
        }

        /// <summary>
        /// Définit l'état de sélection de la cellule.
        /// </summary>
        /// <param name="selected">Indique si la cellule est sélectionnée.</param>
        public void SetIsSelected(bool selected)
        {
            isSelected = selected;
        }

        /// <summary>
        /// Définit l'occupant de la cellule.
        /// </summary>
        /// <param name="occupant">L'occupant de la cellule.</param>
        public void SetOccupant(CellOccupant occupant)
        {
            this.occupant = occupant;
            isOccupied = occupant != null;
        }

        /// <summary>
        /// Définit l'état d'obstacle de la cellule.
        /// </summary>
        /// <param name="obstacle">Indique si la cellule est un obstacle.</param>
        public void SetIsObstacle(bool obstacle)
        {
            isObstacle = obstacle;
        }

        public override string ToString()
        {
            return $"Cell({coordinates.x}, {coordinates.y}, Occupant: {occupant})";
        }
    }
}