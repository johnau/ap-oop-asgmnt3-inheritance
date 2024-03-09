
namespace TaskManagerCore.Infrastructure.BinaryFile.Dao
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

        readonly Dictionary<string, T> Cache = new Dictionary<string, T>();
        readonly Dictionary<string, Func<Dictionary<string, T>, Task>> subscribers;

        public SubscribeableCache() {
            subscribers = new Dictionary<string, Func<Dictionary<string, T>, Task>>();
        }

        public Dictionary<string, T>.ValueCollection Values => Cache.Values;
        public Dictionary<string, T>.Enumerator GetEnumerator() => Cache.GetEnumerator();
        public bool TryGetValue(string key, out T? value) => Cache.TryGetValue(key, out value);
        public bool ContainsKey(string key) => Cache.ContainsKey(key);

        public bool TryAdd(string id, T item)
        {
            if (Cache.TryAdd(id, item)) {
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

        public bool Update(string id, T item)
        {
            if (Cache.TryGetValue(id, out T? existing))
            {
                Cache[id] = item;
                NotifySubscribers(Action.UPDATE);
                return true;
            }

            return false;
        }

        public void Subscribe(Func<Dictionary<string, T>, Task> subscriber)
        {
            var id = Guid.NewGuid().ToString();
            subscribers.TryAdd(id, subscriber);
        }

        void NotifySubscribers(Action action)
        {
            foreach (var item in subscribers)
            {
                var func = item.Value;
                func.Invoke(new Dictionary<string, T>(Cache));
            }
        }
    }
}
