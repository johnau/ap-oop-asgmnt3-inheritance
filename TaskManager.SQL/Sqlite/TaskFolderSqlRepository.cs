using TaskManagerCore.Model.Repository;
using TaskManagerCore.Model;
using TaskManagerCore.SQL.Sqlite.Dao;
using System.Collections.Generic;
using System;

namespace TaskManagerCore.SQL.Sqlite
{
    public class TaskFolderSqlRepository : ITaskFolderRepository
    {
        readonly TaskFolderSqlDao Dao;
        public TaskFolderSqlRepository(TaskFolderSqlDao dao)
        {
            Dao = dao;
        }

        public List<TaskFolder> FindAll()
        {
            var all = Dao.FindAll();
            return EntityFactoryV2.ToModel(all);
        }

        public List<TaskFolder> FindByIds(List<string> ids)
        {
            var matching = Dao.FindByIds(ids);
            return EntityFactoryV2.ToModel(matching);
        }

        public TaskFolder FindById(string id)
        {
            var one = Dao.FindById(id);
            if (one == null) return null;

            return EntityFactoryV2.ToModel(one);
        }

        public TaskFolder Save(TaskFolder o)
        {
            if (Dao.Save(EntityFactoryV2.FromModel(o)))
            {
                return o;
            }

            throw new Exception("Sql error: Unable to add folder");
        }

        public bool Delete(string id)
        {
            return Dao.Delete(id);
        }

        #region TaskFolder specific methods
        public List<TaskFolder> FindByNameStartsWith(string name)
        {
            var results = Dao.FindByNameStartsWith(name);

            return EntityFactoryV2.ToModel(results);
        }

        public TaskFolder FindOneByName(string name)
        {
            var result = Dao.FindByName(name);
            if (result == null)
                return null;

            return EntityFactoryV2.ToModel(result);
        }

        public List<TaskFolder> FindEmpty()
        {
            var results = Dao.FindEmpty();

            return EntityFactoryV2.ToModel(results);
        }

        public List<TaskFolder> FindNotEmpty()
        {
            var results = Dao.FindNotEmpty();

            return EntityFactoryV2.ToModel(results);
        }

        public TaskFolder FindByName(string name)
        {
            var result = Dao.FindByName(name);
            if (result == null)
                return null;

            return EntityFactoryV2.ToModel(result);
        }

        public bool DeleteByName(string name)
        {
            var result = Dao.FindByName(name);
            if (result == null)
                return false;

            return Delete(result.GlobalId);
        }

        #endregion
    }
}
