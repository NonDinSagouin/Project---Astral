namespace Core.Event
{
    /// <summary>
    /// Classe d'événement représentant l'événement OnSelectedCellChange.
    /// Cet événement est déclenché quand la cellule sélectionnée change.
    /// </summary>
    public readonly struct OnSelectedCellChangeEvent
    {
        public object OldCell { get; }
        public object NewCell { get; }

        public OnSelectedCellChangeEvent(object oldCell, object newCell)
        {
            OldCell = oldCell;
            NewCell = newCell;
        }
    }
}
    