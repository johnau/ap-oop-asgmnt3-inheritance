﻿using BinaryFileHandler;
using System.Diagnostics;
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
        where T : EntityBase
    {
        readonly BinaryFileReader<T> Reader;
        readonly BinaryFileWriter<T> Writer;

        //readonly Dictionary<string, T> Cache;
        protected readonly SubscribeableCache<T> Cache;

        protected AbstractDao(BinaryFileReader<T> reader, BinaryFileWriter<T> writer)
        {
            Reader = reader;
            Writer = writer;
            //Cache = new Dictionary<string, T>();
            Cache = new SubscribeableCache<T>();
            LoadData();
            Cache.Subscribe(async (data) =>
            {
                Debug.WriteLine($"Data has been updated notification! {data.Count}");
                Writer.AddObjectsToWrite(new List<T>(data.Values));
                await Writer.WriteValuesAsync();
            });
        }

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
            } catch (Exception ex)
            {
                Debug.WriteLine("There is no data to load");
            }
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
            if (entity == null) throw new ArgumentNullException();

            return Delete(entity.Id);
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
