using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Combat.Grid
{
    public class CellOccupant : MonoBehaviour
    {
        // ----- Private Properties -------------------------
        [SerializeField] private Movement movement;
        [SerializeField] private bool isObstacle;

        private Cell currentCell;

        // ----- Public Properties -------------------------
        public bool IsObstacle => isObstacle;
        public Cell CurrentCell => currentCell;

        /// <summary>
        /// Définit la cellule actuelle de l'occupant.
        /// </summary>
        /// <param name="cell">La cellule à définir comme actuelle.</param>
        public void SetCurrentCell(Cell cell)
        {
            currentCell = cell;
        }

        /// <summary>
        /// Déplace l'occupant vers la cellule spécifiée en utilisant le pathfinder fourni.
        /// </summary>
        /// <param name="cell">La cellule vers laquelle déplacer l'occupant.</param>
        /// <param name="pathfinder">Le pathfinder utilisé pour calculer le chemin.</param>
        public void MoveOccupantToCell(Cell cell, GridPathfinder pathfinder)
        {
            if (cell == null) return;

            if (pathfinder == null || currentCell == null)
            {
                Debug.LogWarning("Déplacement impossible sans cellule actuelle et pathfinder.", this);
                return;
            }

            // Calcul du chemin vers la cellule cible.
            List<Cell> path = pathfinder.FindPath(currentCell, cell);
            if (path == null) return;

            // Conversion du chemin en positions dans le monde.
            List<Vector3> positions = new List<Vector3>(path.Count);
            foreach (Cell pathCell in path)
                positions.Add(pathCell.WorldPosition);

            // Déplacement de l'occupant le long du chemin calculé.
            movement.MoveToPath(positions);
            SetCurrentCell(cell);
        }
    }
}