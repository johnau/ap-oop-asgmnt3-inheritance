namespace TaskManagerCore.Configuration
{
    /// <summary>
    /// Wraps Dictionary to provide subscription
    /// It is not done very safely.
    /// Should just extend the dictionary class? - Will this be frustrating because all methods will be exposed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SubscribeableCache<T>
    {
        protected readonly Dictionary<string, T> Cache; // remvoed protected for a sec, testing something, need to put it back ,we wont change the cache
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

        public virtual bool TryAdd(string id, T item)
        {
            if (Cache.TryAdd(id, item))
            {
                NotifySubscribers(NotifiedAction.ADD);
                return true;
            }

            return false;
        }

        public virtual bool Remove(string id)
        {
            var removed = Cache.Remove(id);
            if (removed)
            {
                NotifySubscribers(NotifiedAction.REMOVE);
                return true;
            }

            return false;
        }

        public virtual bool Flush()
        {
            NotifySubscribers(NotifiedAction.UPDATE);
            return true;
        }

        /// <summary>
        /// Try to avoid using this method
        /// To be removed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        internal virtual bool ForceReplace(string id, T item)
        {
            if (Cache.TryGetValue(id, out T? existing))
            {
                Cache[id] = item; // lazy - should be updating existing
                NotifySubscribers(NotifiedAction.UPDATE);
                return true;
            }

            return false;
        }

        public void Subscribe(Func<Dictionary<string, T>, Task> subscriber)
        {
            var id = Guid.NewGuid().ToString();
            Subscribers.TryAdd(id, subscriber);
        }

        void NotifySubscribers(NotifiedAction action)
        {
            foreach (var item in Subscribers)
            {
                var func = item.Value;
                func.Invoke(new Dictionary<string, T>(Cache));
            }
        }
    }
}
