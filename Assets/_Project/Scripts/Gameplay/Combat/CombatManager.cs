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

        private void OnSelectedCellChange(OnSelectedCellChangeEvent evt)
        {
            gridManager.Pathfinder.ClearPreviewPath();
            Cell destination = evt.NewCell as Cell;
            if (occupantSelected == null || destination == null) return;
            occupantSelected.MoveOccupantToCell(destination, gridManager.Pathfinder);
        }

        private void OnOccupantSelectedChange(OnOccupantSelectedChangeEvent evt)
        {
            if (occupantSelected == gridManager.OccupantSelected) return;

            occupantSelected = gridManager.OccupantSelected;
            gridManager.Pathfinder.ClearPreviewPath();
            selectedCellText.text = occupantSelected != null ? occupantSelected.name : "None";
        }

        private void OnHoveredCellChange(OnHoveredCellChangeEvent evt)
        {
            if (occupantSelected == null || occupantSelected.CurrentCell == null)
            {
                gridManager.Pathfinder.ClearPreviewPath();
                return;
            }

            Cell destination = evt.NewCell as Cell;
            if (destination == null)
            {
                gridManager.Pathfinder.ClearPreviewPath();
                return;
            }

            var path = gridManager.Pathfinder.FindPath(occupantSelected.CurrentCell, destination);
            if (path == null)
            {
                gridManager.Pathfinder.ClearPreviewPath();
                return;
            }

            path.Insert(0, occupantSelected.CurrentCell);
            gridManager.Pathfinder.SetPreviewPath(path);
        }
    }
}