using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Combat.Grid
{
    public class GridPathfinder
    {
        // ----- Private Properties -------------------------
        private readonly Cell[,] grid;
        private List<Cell> previewPath;

        // ----- Public Properties -------------------------

        /// <summary>
        /// Obtient le chemin prévisualisé actuel, s'il existe.
        /// </summary>
        public IReadOnlyList<Cell> PreviewPath => previewPath;

        /// <summary>
        /// Initialise le pathfinder avec la grille spécifiée.
        /// </summary>
        /// <param name="grid">La grille de cellules à utiliser pour le pathfinding.</param>
        public GridPathfinder(Cell[,] grid)
        {
            if (grid == null) throw new System.ArgumentNullException(nameof(grid));
            
            this.grid = grid;
        }

        /// <summary>
        /// Définit le chemin prévisualisé.
        /// </summary>
        /// <param name="path">La liste des cellules formant le chemin à prévisualiser.</param>
        public void SetPreviewPath(List<Cell> path)
        {
            previewPath = path;
        }

        /// <summary>
        /// Efface le chemin prévisualisé.
        /// </summary>
        public void ClearPreviewPath()
        {
            previewPath = null;
        }

        /// <summary>
        /// Calcule le chemin le plus court entre la cellule de départ et la cellule de destination en évitant les obstacles.
        /// </summary>
        /// <param name="start">La cellule de départ.</param>
        /// <param name="destination">La cellule de destination.</param>
        /// <param name="maxMovement">Nombre maximal de cases du chemin. Une valeur négative signifie aucune limite.</param>
        /// <returns>La liste des cellules formant le chemin le plus court, ou null si aucun chemin n'est possible dans la limite de mouvement.</returns>
        public List<Cell> FindPath(Cell start, Cell destination, int maxMovement = -1)
        {
            bool IsCellInGrid(Cell cell, Vector2Int coordinates, int width, int height)
            {
                return coordinates.x >= 0
                    && coordinates.x < width
                    && coordinates.y >= 0
                    && coordinates.y < height
                    && grid[coordinates.x, coordinates.y] == cell;
            }

            if (start == null || destination == null) return null;

            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int startCoordinates = start.Coordinates;
            Vector2Int destinationCoordinates = destination.Coordinates;

            if (!IsCellInGrid(start, startCoordinates, width, height)
                || !IsCellInGrid(destination, destinationCoordinates, width, height))
                return null;

            if (destination.IsObstacle && start != destination)
                return null;

            // Une recherche en largeur garantit le chemin le plus court quand chaque déplacement a le même coût.
            bool[,] visited = new bool[width, height];
            // Mémorise la case précédente pour reconstruire le chemin une fois la destination atteinte.
            Vector2Int[,] previous = new Vector2Int[width, height];
            // Nombre de pas depuis le départ, pour respecter la limite de mouvement.
            int[,] distance = new int[width, height];
            Queue<Vector2Int> cellsToVisit = new Queue<Vector2Int>();
            Vector2Int[] directions =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            visited[startCoordinates.x, startCoordinates.y] = true;
            cellsToVisit.Enqueue(startCoordinates);

            while (cellsToVisit.Count > 0)
            {
                Vector2Int current = cellsToVisit.Dequeue();
                if (current == destinationCoordinates)
                    break;

                // Ne pas étendre au-delà de la limite de mouvement.
                if (maxMovement >= 0 && distance[current.x, current.y] >= maxMovement)
                    continue;

                foreach (Vector2Int direction in directions)
                {
                    Vector2Int next = current + direction;
                    
                    // Ignore les cellules en dehors de la grille.
                    if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                        continue;

                    // Ignore les cellules déjà visitées ou les obstacles.
                    if (visited[next.x, next.y] || grid[next.x, next.y].IsObstacle)
                        continue;

                    visited[next.x, next.y] = true;
                    previous[next.x, next.y] = current;
                    distance[next.x, next.y] = distance[current.x, current.y] + 1;
                    cellsToVisit.Enqueue(next);
                }
            }

            // Une destination jamais visitée est inaccessible depuis le départ.
            if (!visited[destinationCoordinates.x, destinationCoordinates.y])
                return null;

            // Remonte les prédécesseurs jusqu'au départ, puis inverse l'ordre pour obtenir le chemin.
            List<Cell> path = new List<Cell>();
            Vector2Int pathCoordinates = destinationCoordinates;
            while (pathCoordinates != startCoordinates)
            {
                path.Add(grid[pathCoordinates.x, pathCoordinates.y]);
                pathCoordinates = previous[pathCoordinates.x, pathCoordinates.y];
            }

            path.Reverse();
            return path;
        }
    }
}
