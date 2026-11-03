namespace Core.Event
{
    /// <summary>
    /// Classe d'événement représentant l'événement OnHoveredCellChange.
    /// Cet événement est déclenché quand la cellule survolée change.
    /// </summary>
    public readonly struct OnHoveredCellChangeEvent
    {
        public object OldCell { get; }
        public object NewCell { get; }

        public OnHoveredCellChangeEvent(object oldCell, object newCell)
        {
            OldCell = oldCell;
            NewCell = newCell;
        }
    }
}
    