using UnityEngine;

namespace Gameplay.Combat.Grid
{
    public class CellOccupant : MonoBehaviour
    {
        [SerializeField] private Movement movement;

        private Cell currentCell;

        public void SetCurrentCell(Cell cell)
        {
            currentCell = cell;
        }

        public void MoveOccupantToCell(Cell cell)
        {
            SetCurrentCell(cell);
            movement.MoveTo(cell.WorldPosition);
        }
    }
}