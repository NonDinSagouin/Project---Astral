using UnityEngine;

using Core.Manager;
using Helper.Debug;

namespace Gameplay.Combat.Grid
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private bool debugMode;
        [SerializeField, Min(1)] private int gridWidth = 1;
        [SerializeField, Min(1)] private int gridHeight = 1;
        [SerializeField, Min(0.01f)] private float cellSize = 1f;

        private Cell[,] grid;
        private Cell hoveredCell;

        void OnValidate()
        {
            SetUpGrid();
        }
        
        void Awake()
        {
            SetUpGrid();
        }

        void Update()
        {
            MouseManager mouseManager = MouseManager.Instance;
            if (mouseManager == null || !mouseManager.HasWorldMousePosition)
            {
                return;
            }

            /// Get the cell at the current mouse world position.
            Cell cell = GetCellAtWorldPosition(mouseManager.WorldMousePosition);

            if (cell == null)
            {
                hoveredCell = null;
                return;
            }

            if (hoveredCell == cell)
            {
                return;
            }

            /// Set all other cells to not hovered.
            foreach (var c in grid)
            {
                c.SetHovered(c == cell);
                if (c == cell) hoveredCell = c;
            }
        }

        private void SetUpGrid()
        {
            grid = new Cell[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3 cellPosition = new Vector3(
                        (x - (gridWidth - 1) * 0.5f) * cellSize,
                        (y - (gridHeight - 1) * 0.5f) * cellSize,
                        0);

                    Cell cell = new Cell(new Vector2Int(x, y), cellPosition);
                    grid[x, y] = cell;
                }
            }
        }

        private Cell GetCellAtWorldPosition(Vector3 worldPosition)
        {
            Vector3 localPosition = transform.InverseTransformPoint(worldPosition);
            int x = Mathf.FloorToInt(localPosition.x / cellSize + gridWidth * 0.5f);
            int y = Mathf.FloorToInt(localPosition.y / cellSize + gridHeight * 0.5f);

            if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return null;
            return grid[x, y];
        }
        
        void OnDrawGizmos()
        {
            if (grid == null) return;
            
            Matrix4x4 previousMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            foreach (var cell in grid)
            {
                Vector3 cellPosition = cell.WorldPosition;

                if (cell.IsHovered)
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = Color.white;
                }

                Gizmos.DrawWireCube(cellPosition, Vector3.one * cellSize);
            }

            Gizmos.matrix = previousMatrix;
        }
    }
}