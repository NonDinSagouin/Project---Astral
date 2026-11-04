using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Combat.Grid
{
    public class GridPathfinder
    {
        private readonly Cell[,] grid;
        private List<Cell> previewPath;

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
        /// <returns>La liste des cellules formant le chemin le plus court, ou null si aucun chemin n'est possible.</returns>
        public List<Cell> FindPath(Cell start, Cell destination)
        {
            if (start == null || destination == null)
                return null;

            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            Vector2Int startCoordinates = start.Coordinates;
            Vector2Int destinationCoordinates = destination.Coordinates;

            if (!IsCellInGrid(start, startCoordinates, width, height)
                || !IsCellInGrid(destination, destinationCoordinates, width, height))
                return null;

            if (destination.IsObstacle && start != destination)
                return null;

            bool[,] visited = new bool[width, height];
            Vector2Int[,] previous = new Vector2Int[width, height];
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

                foreach (Vector2Int direction in directions)
                {
                    Vector2Int next = current + direction;
                    if (next.x < 0 || next.x >= width || next.y < 0 || next.y >= height)
                        continue;
                    if (visited[next.x, next.y] || grid[next.x, next.y].IsObstacle)
                        continue;

                    visited[next.x, next.y] = true;
                    previous[next.x, next.y] = current;
                    cellsToVisit.Enqueue(next);
                }
            }

            if (!visited[destinationCoordinates.x, destinationCoordinates.y])
                return null;

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

        private bool IsCellInGrid(Cell cell, Vector2Int coordinates, int width, int height)
        {
            return coordinates.x >= 0
                && coordinates.x < width
                && coordinates.y >= 0
                && coordinates.y < height
                && grid[coordinates.x, coordinates.y] == cell;
        }    
    }
}
