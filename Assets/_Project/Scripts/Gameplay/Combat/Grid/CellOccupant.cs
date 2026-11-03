using UnityEngine;

namespace Gameplay.Combat.Grid
{
    public class CellOccupant : MonoBehaviour
    {
        private Cell currentCell;

        public void SetCurrentCell(Cell cell)
        {
            currentCell = cell;
        }

        public void MoveOccupantToCell(Cell cell)
        {
            SetCurrentCell(cell);
            transform.position = cell.WorldPosition;
        }
    }
}