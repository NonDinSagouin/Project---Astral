using UnityEngine;
using Core;
using Core.Manager;
using Core.Event;

namespace Gameplay.Combat.Grid
{
    public class GridManager : MonoBehaviour
    {
        // ----- Private Properties -------------------------
        [SerializeField] private bool debugMode;
        [SerializeField, Min(1)] private int gridWidth = 1;
        [SerializeField, Min(1)] private int gridHeight = 1;
        [SerializeField, Min(0.01f)] private float cellSize = 1f;

        [SerializeField, Min(0.01f)] private float collisionCheckSize = 0.1f;
        [SerializeField] private float occupiedPointRadius = 0.1f;

        private MouseManager mouseManager;
        private Cell[,] grid;
        private GridPathfinder pathfinder;

        private Cell hoveredCell;
        private Cell selectedCell;
        private CellOccupant occupantSelected;

        // ----- Public Properties -------------------------
        public Cell SelectedCell => selectedCell;
        public Cell HoveredCell => hoveredCell;
        public CellOccupant OccupantSelected => occupantSelected;
        public GridPathfinder Pathfinder => pathfinder;

        void OnValidate()
        {
            SetUpGrid();
        }
        
        void Awake()
        {
            SetUpGrid();
        }

        void Start()
        {
            mouseManager = MouseManager.Instance;
        }

        void Update()
        {
            if (mouseManager == null) return;

            // Gestion du survol des cellules.
            HoverHandler();

            // Gestion de la sélection des cellules.
            SelectHandler();

            // Met à jour les paramètres de toutes les cellules.
            UpdateCellsParameters();
        }

        /// <summary>
        /// Récupère la cellule aux coordonnées spécifiées.
        /// </summary>
        /// <param name="coordinates">Les coordonnées de la cellule à récupérer.</param>
        /// <returns>La cellule aux coordonnées spécifiées.</returns>
        public Cell GetCell(Vector2Int coordinates)
        {
            if (coordinates.x < 0 || coordinates.x >= gridWidth || coordinates.y < 0 || coordinates.y >= gridHeight)
            {
                throw new System.Exception("Coordinates out of grid bounds ! " + coordinates.x + ", " + coordinates.y);
            }

            if (grid[coordinates.x, coordinates.y] == null)
            {
                throw new System.Exception("Cell at coordinates is not initialized ! " + coordinates.x + ", " + coordinates.y);
            }

            Cell cell = grid[coordinates.x, coordinates.y];

            return cell;
        }

        /// <summary>
        /// Gestion de l'état de survol des cellules.
        /// </summary>
        private void HoverHandler()
        {
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

            var oldCell = hoveredCell;
            hoveredCell = cell;
            EventBus.Publish(new OnHoveredCellChangeEvent(oldCell, hoveredCell));
        }

        /// <summary>
        /// Gestion de l'état de sélection des cellules.
        /// </summary>
        private void SelectHandler()
        {
            if (!mouseManager.WasLeftMousePressedThisFrame) return;

            var oldCell = selectedCell;
            var oldOccupant = occupantSelected;

            selectedCell = hoveredCell;
            occupantSelected = hoveredCell != null ? hoveredCell.Occupant : null;

            if (oldCell != selectedCell)
                EventBus.Publish(new OnSelectedCellChangeEvent(oldCell, selectedCell));

            if (oldOccupant != occupantSelected)
                EventBus.Publish(new OnOccupantSelectedChangeEvent(oldOccupant, occupantSelected));
        }

        /// <summary>
        /// Met à jour les paramètres de toutes les cellules.
        /// </summary>
        private void UpdateCellsParameters()
        {
            foreach (var cell in grid)
            {
                SetCellOccupant(cell);

                // Met à jour l'état de survol et de sélection de la cellule.
                cell.SetIsHovered(cell == hoveredCell);

                // Met à jour l'état de sélection de la cellule.
                cell.SetIsSelected(cell == selectedCell);
            }
        }

        /// <summary>
        /// Définit l'occupant de la cellule en fonction de la présence d'un collider à sa position.
        /// </summary>
        /// <param name="cell">La cellule dont l'occupant doit être défini.</param>
        private void SetCellOccupant(Cell cell)
        {
            // Vérifie s'il y a un occupant de cellule à la position de la cellule.
            Collider2D CellCollider = Physics2D.OverlapBox(cell.WorldPosition, Vector2.one * collisionCheckSize, 0f);

            if (CellCollider != null)
            {
                CellOccupant occupant = CellCollider.GetComponent<CellOccupant>();

                if (occupant == null) return;
                if (occupant == cell.Occupant) return;

                cell.SetOccupant(occupant);
                cell.SetIsObstacle(occupant.IsObstacle);
                occupant.SetCurrentCell(cell);
            }
            else
            {
                cell.SetOccupant(null);
                cell.SetIsObstacle(false);
            }
        }

        /// <summary>
        /// Initialise la grille de cellules.
        /// </summary>
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

            pathfinder = new GridPathfinder(grid);
        }

        /// <summary>
        /// Obtient la cellule à la position du monde spécifiée.
        /// </summary>
        /// <param name="worldPosition">La position dans le monde.</param>
        /// <returns>La cellule correspondante ou null si en dehors de la grille.</returns>
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
            
            /// Dessine la grille et les cellules avec leurs états.
            /// Les couleurs représentent les différents états des cellules.
            foreach (var cell in grid)
            {
                Gizmos.color = Color.white;
                Vector3 cellPosition = cell.WorldPosition;
                Gizmos.DrawWireCube(cellPosition, Vector3.one * cellSize);

                Gizmos.color = Color.white;
                if (cell.IsHovered) Gizmos.color = Color.yellow;
                if (cell.IsSelected) Gizmos.color = Color.green;
                if (cell.IsOccupied) Gizmos.color = Color.red;
                if (cell.IsObstacle) Gizmos.color = Color.black;
                
                Gizmos.DrawSphere(cell.WorldPosition, occupiedPointRadius);
            }

            /// Dessine le chemin prévisualisé si disponible.
            /// Les lignes cyan représentent le chemin prévisualisé.
            if (pathfinder != null && pathfinder.PreviewPath != null)
            {
                Gizmos.color = Color.cyan;
                for (int i = 1; i < pathfinder.PreviewPath.Count; i++)
                {
                    Vector3 start = pathfinder.PreviewPath[i - 1].WorldPosition;
                    Vector3 end = pathfinder.PreviewPath[i].WorldPosition;
                    start.z -= 0.01f;
                    end.z -= 0.01f;
                    Gizmos.DrawLine(start, end);
                    Gizmos.DrawSphere(end, occupiedPointRadius * 0.5f);
                }
            }

            Gizmos.matrix = previousMatrix;
        }
    }
}