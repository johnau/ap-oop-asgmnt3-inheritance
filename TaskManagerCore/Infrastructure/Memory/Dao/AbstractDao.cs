using System.ComponentModel;
using System.Diagnostics;
using TaskManagerCore.Configuration;
using TaskManagerCore.Infrastructure.Memory.Entity;

namespace TaskManagerCore.Infrastructure.Memory.Dao
{
    public abstract class AbstractDao<T> : ICrudRepository<T, string>
        where T : EntityBase
    {
        protected Dictionary<string, T> Cache;

        protected AbstractDao()
        {
            Cache = new Dictionary<string, T>();
        }

        public List<T> FindAll()
        {
            return new List<T>(Cache.Values);
        }

        public List<T> FindByIds(List<string> ids)
        {
            List<T> matching = new List<T>();
            foreach (var item in Cache)
            {
                if (ids.Contains(item.Key)) 
                {
                    matching.Add(item.Value);
                }
            }
            return matching;
        }

        public virtual T? FindById(string id)
        {
            Cache.TryGetValue(id, out T? entity);
            return entity;
        }

        public virtual bool Delete(T entity)
        {
            if (Cache.ContainsKey(entity.Id))
            {
                Debug.WriteLine($"Deleting task: {entity.Id}");
                Cache.Remove(entity.Id);
                return true;
            }

            Debug.WriteLine($"Can't remove task: {entity.Id}. It does not exist");
            return false;
        }

        public abstract string Save(T entity);

        public virtual bool Delete(string id)
        {
            if (Cache.ContainsKey(id))
            {
                Debug.WriteLine($"Deleting {typeof(T).Name}: {id}");
                return Cache.Remove(id);
            }

            Debug.WriteLine($"Can't remove {typeof(T).Name} with Id={id}. It does not exist");
            return false;
        }

    }
}
