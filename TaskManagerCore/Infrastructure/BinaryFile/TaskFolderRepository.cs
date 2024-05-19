using System.Collections.Generic;
using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    internal class TaskFolderRepository : ITaskFolderRepository
    {
        readonly TaskFolderDao Dao;
        internal TaskFolderRepository(TaskFolderDao dao)
        {
            Dao = dao;
        }

        public List<TaskFolder> FindAll()
        {
            var all = Dao.FindAll();
            return EntityFactory.ToModel(all);
        }

        public List<TaskFolder> FindByIds(List<string> ids)
        {
            var matching = Dao.FindByIds(ids);
            return EntityFactory.ToModel(matching);
        }

        public TaskFolder FindById(string id)
        {
            var one = Dao.FindById(id);
            if (one == null) return null;
            
            return EntityFactory.ToModel(one);
        }

        public TaskFolder Save(TaskFolder o)
        {
            var saved = Dao.Save(EntityFactory.FromModel(o));
            return EntityFactory.ToModel(saved);
        }

        public bool Delete(string id)
        {
            return Dao.Delete(id);
        }

        #region TaskFolder specific methods
        public List<TaskFolder> FindByNameStartsWith(string name)
        {
            var results = Dao.FindByName(name);
            return EntityFactory.ToModel(results);
        }

        public TaskFolder FindOneByName(string name)
        {
            var results = Dao.FindOneByName(name);
            if (results == null) return null;

            return EntityFactory.ToModel(results);
        }

        public List<TaskFolder> FindEmpty()
        {
            var results = Dao.FindEmpty();
            return EntityFactory.ToModel(results);
        }

        public List<TaskFolder> FindNotEmpty()
        {
            var results = Dao.FindNotEmpty();
            return EntityFactory.ToModel(results);
        }

        public TaskFolder FindByName(string name)
        {
            var result = Dao.FindOneByName(name);
            if (result == null)
                return null;

            return EntityFactory.ToModel(result);
        }

        public bool DeleteByName(string name)
        {
            var result = Dao.FindOneByName(name);
            if (result == null)
                return false;

            return Dao.Delete(result.Id);
        }

        #endregion
    }
}
