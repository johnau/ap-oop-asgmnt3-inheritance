namespace TaskManagerCore.Configuration
{
    /// <summary>
    /// Wraps Dictionary to provide subscription (extending or implementing exposes too many methods)
    /// TODO: Some unsafe-ish things going on need to be fixed at some point
    /// 
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

        #region Dictionary methods
        // Providing these methods so that this Cache can be easily substituted where a Dictionary is currently used
        // If enough are implemented, might as well just implement IDictionary, IEnumerable, etc and override all

        /// <summary>
        /// Simulating a method from Dictionary class that was in use, since this class
        /// has replaced a Dictionary in the DAO objects
        /// </summary>
        public Dictionary<string, T>.ValueCollection Values => Cache.Values;
        
        /// <summary>
        /// Simulating a method from Dictionary class that was in use, since this class
        /// has replaced a Dictionary in the DAO objects
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, T>.Enumerator GetEnumerator() => Cache.GetEnumerator();
        
        /// <summary>
        /// Simulating a method from Dictionary class that was in use, since this class
        /// has replaced a Dictionary in the DAO objects
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(string key, out T? value) => Cache.TryGetValue(key, out value);
        
        /// <summary>
        /// Simulating a method from Dictionary class that was in use, since this class
        /// has replaced a Dictionary in the DAO objects
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(string key) => Cache.ContainsKey(key);

        /// <summary>
        /// Simulating a method from Dictionary class that was in use, since this class
        /// has replaced a Dictionary in the DAO objects
        /// </summary>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool TryAdd(string id, T item)
        {
            if (Cache.TryAdd(id, item))
            {
                NotifySubscribers(NotifiedAction.ADD);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Simulating a method from Dictionary class that was in use, since this class
        /// has replaced a Dictionary in the DAO objects
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        #endregion

        /// <summary>
        /// Triggers an Update of the Cache
        /// </summary>
        public virtual void MarkDirty()
        {
            NotifySubscribers(NotifiedAction.UPDATE);
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
                NotifySubscribers(NotifiedAction.REMOVE); // safer to fire both than `UPDATE`
                NotifySubscribers(NotifiedAction.ADD);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Add a subscriber Func to the Cache
        /// </summary>
        /// <param name="subscriber"></param>
        public void Subscribe(Func<Dictionary<string, T>, Task> subscriber)
        {
            var id = Guid.NewGuid().ToString();
            Subscribers.TryAdd(id, subscriber);
        }

        /// <summary>
        /// Calls all subscriber Funcs
        /// </summary>
        /// <param name="action"></param>
        /// <param name="id"></param>
        protected virtual void NotifySubscribers(NotifiedAction action, string? id = null)
        {
            foreach (var item in Subscribers)
            {
                var func = item.Value;
                func.Invoke(new Dictionary<string, T>(Cache));
            }
        }
    }
}
