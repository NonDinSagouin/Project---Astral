using UnityEngine;
using TMPro;

using Core;
using Core.Event;
using Gameplay.Combat.Grid;

namespace Gameplay.Combat
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private CellOccupant occupantSelected;

        [SerializeField] private TextMeshProUGUI selectedCellText;

        void Start()
        {
            EventBus.Subscribe<OnSelectedCellChangeEvent>(OnSelectedCellChange);
            EventBus.Subscribe<OnHoveredCellChangeEvent>(OnHoveredCellChange);
            EventBus.Subscribe<OnOccupantSelectedChangeEvent>(OnOccupantSelectedChange);
        }

        void OnDestroy()
        {
            EventBus.Unsubscribe<OnSelectedCellChangeEvent>(OnSelectedCellChange);
            EventBus.Unsubscribe<OnHoveredCellChangeEvent>(OnHoveredCellChange);
            EventBus.Unsubscribe<OnOccupantSelectedChangeEvent>(OnOccupantSelectedChange);
        }

        /// <summary>
        /// Evenement déclenché lorsque la cellule sélectionnée change.
        /// </summary>
        /// <param name="evt"></param>
        private void OnSelectedCellChange(OnSelectedCellChangeEvent evt)
        {
            gridManager.Pathfinder.ClearPreviewPath();
            Cell destination = evt.NewCell as Cell;

            // Si l'occupant sélectionné ou la cellule de destination est invalide, on ne fait rien.
            if (occupantSelected == null || destination == null || destination.Occupant != null) return;

            // Déplace l'occupant vers la cellule de destination si elle est valide.
            occupantSelected.MoveOccupantToCell(destination, gridManager.Pathfinder);
        }

        /// <summary>
        /// Evenement déclenché lorsque l'occupant sélectionné change.
        /// </summary>
        /// <param name="evt">L'événement contenant les informations sur le changement de l'occupant sélectionné.</param>
        private void OnOccupantSelectedChange(OnOccupantSelectedChangeEvent evt)
        {
            CellOccupant newOccupant = evt.NewOccupant as CellOccupant;

            // Vérifie si l'occupant sélectionné a réellement changé avant de mettre à jour.
            if (occupantSelected == newOccupant) return;

            // Efface les cellules atteignables précédemment calculées.
            gridManager.Reachability.ClearReachableCells();
            // Met à jour l'occupant sélectionné et nettoie le chemin de prévisualisation.
            gridManager.Pathfinder.ClearPreviewPath();
            occupantSelected = newOccupant;

            // Calcule les cellules atteignables à partir de la cellule du nouvel occupant avec une portée de 4.
            if (occupantSelected != null && occupantSelected.CurrentCell != null)
                gridManager.Reachability.Compute(occupantSelected.CurrentCell, occupantSelected.MaxMovement);

            selectedCellText.text = occupantSelected != null ? occupantSelected.name : "None";
        }

        /// <summary>
        /// Evenement déclenché lorsque la cellule survolée change.
        /// </summary>
        /// <param name="evt">L'événement contenant les informations sur le changement de la cellule survolée.</param>
        private void OnHoveredCellChange(OnHoveredCellChangeEvent evt)
        {
            // Vérifie si l'occupant sélectionné et sa cellule actuelle sont valides avant de continuer.
            if (occupantSelected == null || occupantSelected.CurrentCell == null)
            {
                gridManager.Pathfinder.ClearPreviewPath();
                return;
            }

            // Récupère la cellule de destination à partir de l'événement.
            Cell destination = evt.NewCell as Cell;
            if (destination == null)
            {
                gridManager.Pathfinder.ClearPreviewPath();
                return;
            }

            // Cherche le chemin entre la cellule actuelle de l'occupant et la cellule de destination.
            var path = gridManager.Pathfinder.FindPath(occupantSelected.CurrentCell, destination);
            if (path == null)
            {
                gridManager.Pathfinder.ClearPreviewPath();
                return;
            }

            // Ajoute la cellule actuelle de l'occupant au début du chemin et met à jour le chemin de prévisualisation.
            path.Insert(0, occupantSelected.CurrentCell);
            gridManager.Pathfinder.SetPreviewPath(path);
        }
    }
}