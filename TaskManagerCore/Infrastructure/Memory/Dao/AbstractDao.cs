﻿using System.ComponentModel;
using System.Diagnostics;
using TaskManagerCore.Configuration;
using TaskManagerCore.Infrastructure.Memory.Entity;

namespace TaskManagerCore.Infrastructure.Memory.Dao
{
    public abstract class AbstractDao<T> : ICrudRepository<T, string>
        where T : EntityBase
    {
        protected Dictionary<string, T> InMemoryData;

        protected AbstractDao()
        {
            InMemoryData = new Dictionary<string, T>();
        }

        public List<T> FindAll()
        {
            return new List<T>(InMemoryData.Values);
        }

        public List<T> FindByIds(List<string> ids)
        {
            List<T> matching = new List<T>();
            foreach (var item in InMemoryData)
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
            InMemoryData.TryGetValue(id, out T? entity);
            return entity;
        }

        public virtual bool Delete(T entity)
        {
            if (InMemoryData.ContainsKey(entity.Id))
            {
                Debug.WriteLine($"Deleting task: {entity.Id}");
                InMemoryData.Remove(entity.Id);
                return true;
            }

            Debug.WriteLine($"Can't remove task: {entity.Id}. It does not exist");
            return false;
        }

        public abstract string Save(T entity);

        public virtual bool Delete(string id)
        {
            if (InMemoryData.ContainsKey(id))
            {
                Debug.WriteLine($"Deleting {typeof(T).Name}: {id}");
                return InMemoryData.Remove(id);
            }

            Debug.WriteLine($"Can't remove {typeof(T).Name} with Id={id}. It does not exist");
            return false;
        }

    }
}
