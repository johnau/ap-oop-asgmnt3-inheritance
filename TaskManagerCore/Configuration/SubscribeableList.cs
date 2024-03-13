namespace TaskManagerCore.Configuration
{
    
    internal class SubscribeableList<T> : List<T>
    {
        private static List<T> EmptyList = new List<T>();
        //protected readonly List<T> Cache;
        protected readonly Dictionary<string, Func<List<T>, Task>> Subscribers;

        // restructure ugly constructors
        public SubscribeableList()
            :this(EmptyList)
        {
        }

        // reimplementing this cache as a list only causes problems for the FindById's method, which can be refactored easily

        public SubscribeableList(IEnumerable<T> collection) 
            : base(collection)
        {
            Subscribers = new Dictionary<string, Func<List<T>, Task>>();
        }

        public IEnumerable<T> Values => this;

        public void TryAdd(string id, T item)
        {
            NotifySubscribers(NotifiedAction.ADD);
            throw new NotImplementedException();
            
        }

        public void TryGetValue(string key, out T value) {  
            throw new NotImplementedException(); 
        }

        public bool ContainsKey(string key) 
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            NotifySubscribers(NotifiedAction.REMOVE);
            throw new NotImplementedException();
            
        }

        public bool Flush()
        {
            NotifySubscribers(NotifiedAction.UPDATE);
            return true;
        }

        public void Subscribe(Func<List<T>, Task> subscriber)
        {
            var id = Guid.NewGuid().ToString();
            Subscribers.TryAdd(id, subscriber);
        }

        void NotifySubscribers(NotifiedAction action)
        {
            foreach (var item in Subscribers)
            {
                var func = item.Value;
                func.Invoke(new List<T>(this));
            }
        }
    }
}
