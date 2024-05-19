using BinaryFileHandler;
using InMemoryCache;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TaskManagerCore.Configuration;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.Dao
{
    /// <summary>
    /// This infrastructure is the same as the Memory namespace, just the Subscribeable cache has been added in place of the dictionary Cache.
    /// This allows the Binary File Writers to be subscribed to updates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class AbstractDao<T> : ICrudRepository<T, string>
        where T : EntityBase, IComparable<T>, ISearchable
    {
        readonly BinaryFileReader<T> Reader;
        readonly BinaryFileWriter<T> Writer;

        protected readonly SortableSubscribeableCache<T> Cache;

        protected AbstractDao(BinaryFileReader<T> reader, BinaryFileWriter<T> writer)
        {
            Reader = reader;
            Writer = writer;
            Cache = new SortableSubscribeableCache<T>(ComparisonMethods);
            LoadData();

            // Subscribe to Cache updates for BinaryFile Writes
            Cache.Subscribe(WriteUpdateData); 
        }

        /// <summary>
        /// Method that provides methods used for sorting by the Type specified in the concrete Class
        /// </summary>
        protected abstract Dictionary<string, Comparison<T>> ComparisonMethods { get; }

        /// <summary>
        /// Save an item to Cache
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract T Save(T entity);

        /// <summary>
        /// Write updated data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task WriteUpdateData(List<T> data)
        {
            Debug.WriteLine($"Writing{data.Count} items");
            
            Writer.AddObjectsToWrite(data);
            return Writer.WriteValuesAsync(); 
        }

        /// <summary>
        /// Load data from file
        /// </summary>
        void LoadData()
        {
            try
            {
                var persistentData = Reader.ReadValues();
                foreach (var item in persistentData)
                {
                    Cache.TryAdd(item.Id, item);
                }
                Debug.WriteLine($"Loaded data: {persistentData.Count}");
            } catch (Exception)
            {
                Debug.WriteLine("There is no data to load, first use!");
            }
        }

        /// <summary>
        /// Get all values from Cache
        /// </summary>
        /// <returns></returns>
        public List<T> FindAll()
        {
            return new List<T>(Cache.Values);
        }

        /// <summary>
        /// Find matches with given list of Ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<T> FindByIds(List<string> ids)
        {
            List<T> matching = new List<T>();
            foreach (var id in ids)
            {
                if (Cache.TryGetValue(id, out var match) && match != null)
                {
                    matching.Add(match);
                }
            }
            return matching;
        }

        /// <summary>
        /// Find match with given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T FindById(string id)
        {
            Cache.TryGetValue(id, out T entity);
            return entity;
        }

        /// <summary>
        /// Delete an item from Cache
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual bool Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException();

            return Delete(entity.Id);
        }

        /// <summary>
        /// Delete an item from Cache
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
