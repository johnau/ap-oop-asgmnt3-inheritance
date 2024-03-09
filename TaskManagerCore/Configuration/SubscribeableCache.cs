namespace TaskManagerCore.Configuration
{
    /// <summary>
    /// Wraps Dictionary to provide subscription
    /// It is not done very safely.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SubscribeableCache<T>
    {
        enum Action
        {
            ADD,
            REMOVE,
            UPDATE,
        }

        protected readonly Dictionary<string, T> Cache;
        protected readonly Dictionary<string, Func<Dictionary<string, T>, Task>> Subscribers;

        public SubscribeableCache()
        {
            Cache = new Dictionary<string, T>();
            Subscribers = new Dictionary<string, Func<Dictionary<string, T>, Task>>();
        }

        public Dictionary<string, T>.ValueCollection Values => Cache.Values;
        public Dictionary<string, T>.Enumerator GetEnumerator() => Cache.GetEnumerator();
        public bool TryGetValue(string key, out T? value) => Cache.TryGetValue(key, out value);
        public bool ContainsKey(string key) => Cache.ContainsKey(key);

        public bool TryAdd(string id, T item)
        {
            if (Cache.TryAdd(id, item))
            {
                NotifySubscribers(Action.ADD);
                return true;
            }

            return false;
        }

        public bool Remove(string id)
        {
            var removed = Cache.Remove(id);
            if (removed)
            {
                NotifySubscribers(Action.REMOVE);
                return true;
            }

            return false;
        }

        public bool Flush()
        {
            NotifySubscribers(Action.UPDATE);
            return true;
        }

        /// <summary>
        /// Try to avoid using this method
        /// To be removed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal bool ForceReplace(string id, T item)
        {
            if (Cache.TryGetValue(id, out T? existing))
            {
                Cache[id] = item; // lazy - should be updating existing
                NotifySubscribers(Action.UPDATE);
                return true;
            }

            return false;
        }

        public void Subscribe(Func<Dictionary<string, T>, Task> subscriber)
        {
            var id = Guid.NewGuid().ToString();
            Subscribers.TryAdd(id, subscriber);
        }

        void NotifySubscribers(Action action)
        {
            foreach (var item in Subscribers)
            {
                var func = item.Value;
                func.Invoke(new Dictionary<string, T>(Cache));
            }
        }
    }
}
