using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Combat.Grid
{
    public class GridReachability
    {
        // ----- Private Properties -------------------------
        private static readonly Vector2Int[] Directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        private readonly Cell[,] grid;
        private readonly List<Cell> reachableCells = new List<Cell>();

        // ----- Public Properties -------------------------

        /// <summary>
        /// Cellules atteignables calculées lors du dernier appel à <see cref="Compute"/>.
        /// </summary>
        public List<Cell> ReachableCells => reachableCells;

        /// <summary>
        /// Initialise le calcul de portée avec la grille spécifiée.
        /// </summary>
        /// <param name="grid">La grille de cellules à utiliser.</param>
        public GridReachability(Cell[,] grid)
        {
            this.grid = grid ?? throw new System.ArgumentNullException(nameof(grid));
        }

        /// <summary>
        /// Efface la liste des cellules atteignables.
        /// </summary>
        public void ClearReachableCells()
        {
            reachableCells.Clear();
        }

        /// <summary>
        /// Calcule les cellules atteignables depuis une cellule de départ en au plus <paramref name="maxDistance"/> pas.
        /// Les obstacles bloquent le passage. Les cellules occupées sont traversables mais ne sont pas des destinations valides.
        /// La cellule de départ n'est pas incluse.
        /// </summary>
        /// <param name="start">La cellule de départ.</param>
        /// <param name="maxDistance">Le nombre maximum de déplacements.</param>
        /// <returns>La liste des cellules atteignables.</returns>
        public List<Cell> Compute(Cell start, int maxDistance)
        {
            reachableCells.Clear();

            if (start == null || maxDistance <= 0) return reachableCells;

            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int startCoordinates = start.Coordinates;

            if (!IsInGrid(startCoordinates, width, height) || grid[startCoordinates.x, startCoordinates.y] != start)
                return reachableCells;

            // -1 signifie "non visitée".
            int[,] distances = new int[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    distances[x, y] = -1;

            Queue<Vector2Int> cellsToVisit = new Queue<Vector2Int>();
            distances[startCoordinates.x, startCoordinates.y] = 0;
            cellsToVisit.Enqueue(startCoordinates);

            while (cellsToVisit.Count > 0)
            {
                Vector2Int current = cellsToVisit.Dequeue();
                int currentDistance = distances[current.x, current.y];

                // Les voisins seraient hors de portée.
                if (currentDistance >= maxDistance) continue;

                foreach (Vector2Int direction in Directions)
                {
                    Vector2Int next = current + direction;

                    if (!IsInGrid(next, width, height)) continue;
                    if (distances[next.x, next.y] >= 0) continue;

                    Cell nextCell = grid[next.x, next.y];
                    if (nextCell.IsObstacle) continue;

                    distances[next.x, next.y] = currentDistance + 1;
                    cellsToVisit.Enqueue(next);

                    if (!nextCell.IsOccupied)
                        reachableCells.Add(nextCell);
                }
            }

            return reachableCells;
        }

        private static bool IsInGrid(Vector2Int coordinates, int width, int height)
        {
            return coordinates.x >= 0 && coordinates.x < width
                && coordinates.y >= 0 && coordinates.y < height;
        }
    }
}
