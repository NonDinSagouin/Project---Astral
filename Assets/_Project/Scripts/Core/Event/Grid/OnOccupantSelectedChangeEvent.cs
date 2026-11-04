namespace Core.Event
{
    /// <summary>
    /// Classe d'événement représentant l'événement OnOccupantSelectedChange.
    /// Cet événement est déclenché quand l'occupant sélectionné change.
    /// </summary>
    public readonly struct OnOccupantSelectedChangeEvent
    {
        public object OldOccupant { get; }
        public object NewOccupant { get; }

        public OnOccupantSelectedChangeEvent(object oldOccupant, object newOccupant)
        {
            OldOccupant = oldOccupant;
            NewOccupant = newOccupant;
        }
    }
}
    