using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Combat.Grid
{
    public class CellOccupant : MonoBehaviour
    {
        [SerializeField] private Movement movement;
        [SerializeField] private bool isObstacle;

        private Cell currentCell;

        public bool IsObstacle => isObstacle;
        public Cell CurrentCell => currentCell;

        public void SetCurrentCell(Cell cell)
        {
            currentCell = cell;
        }

        public void MoveOccupantToCell(Cell cell, GridPathfinder pathfinder)
        {
            if (cell == null)
                return;

            if (pathfinder == null || currentCell == null)
            {
                Debug.LogWarning("Cannot move occupant without its current cell and pathfinder.", this);
                return;
            }

            List<Cell> path = pathfinder.FindPath(currentCell, cell);
            if (path == null)
                return;

            SetCurrentCell(cell);

            List<Vector3> positions = new List<Vector3>(path.Count);
            foreach (Cell pathCell in path)
                positions.Add(pathCell.WorldPosition);

            movement.MoveToPath(positions);
        }
    }
}