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
            if (occupantSelected == null) return;
            occupantSelected.MoveOccupantToCell((Cell)evt.NewCell);
        }

        private void OnOccupantSelectedChange(OnOccupantSelectedChangeEvent evt)
        {
            if (occupantSelected == gridManager.OccupantSelected) return;

            occupantSelected = gridManager.OccupantSelected;
            selectedCellText.text = occupantSelected != null ? occupantSelected.name : "None";
        }

        private void OnHoveredCellChange(OnHoveredCellChangeEvent evt)
        {   
            
        }
    }
}