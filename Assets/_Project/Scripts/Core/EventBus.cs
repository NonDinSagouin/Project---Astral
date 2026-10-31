namespace Core
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> events = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            events.Clear();
        }

        /// <summary>
        /// Publie un événement de type T. Tous les abonnés à cet événement seront notifiés.
        /// </summary>
        /// <typeparam name="T">Le type de l'événement à publier.</typeparam>
        public static void Publish<T>(T e)
        {
            if (events.TryGetValue(typeof(T), out var existing))
            {
                (existing as Action<T>)?.Invoke(e);
            }
        }

        /// <summary>
        /// Abonne une action à un événement de type TEventType. Lorsque l'événement est levé, l'action sera exécutée.
        /// </summary>
        /// <param name="eventType">Le type d'événement auquel s'abonner.</param>
        /// <param name="action">L'action à exécuter lorsque l'événement est levé.</param>
        public static void Subscribe<T>(Action<T> callback)
        {
            if (callback == null)
            {
                return;
            }

            if (events.TryGetValue(typeof(T), out var existing))
            {
                events[typeof(T)] = Delegate.Combine(existing, callback);
            }
            else
            {
                events[typeof(T)] = callback;
            }
        }

        /// <summary>
        /// Désabonne une action d'un événement de type TEventType. L'action ne sera plus exécutée lorsque l'événement est levé.
        /// </summary>
        /// <param name="eventType">Le type d'événement dont se désabonner.</param>
        /// <param name="action">L'action à désabonner.</param>
        public static void Unsubscribe<T>(Action<T> callback)
        {
            if (callback == null)
            {
                return;
            }

            if (events.TryGetValue(typeof(T), out var existing))
            {
                Delegate updated = Delegate.Remove(existing, callback);

                if (updated == null)
                {
                    events.Remove(typeof(T));
                }
                else
                {
                    events[typeof(T)] = updated;
                }
            }
        }

    }
}