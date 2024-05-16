using TaskManagerCore.Model.Repository;
using TaskManagerCore.Model;
using TaskManagerCore.Infrastructure.Sqlite.Dao;

namespace TaskManagerCore.Infrastructure.Sqlite
{
    internal class TaskFolderSqlRepository : ITaskFolderRepository
    {
        readonly TaskFolderSqlDao Dao;
        internal TaskFolderSqlRepository(TaskFolderSqlDao dao)
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

        public TaskFolder? FindById(string id)
        {
            var one = Dao.FindById(id);
            if (one == null) return null;

            return EntityFactory.ToModel(one);
        }

        public string Save(TaskFolder o)
        {
            if (Dao.Create(EntityFactory.FromModel(o)))
            {
                return o.Id;
            }

            throw new Exception("Sql error: Unable to add folder");
        }

        public bool Delete(string id)
        {
            var existing = FindById(id);
            if (existing != null)
            {
                existing = existing.WithName("<DELETED>"); // assume folder is empty - given this is a secondary data store right now
                Save(existing);
                return true;
            }

            return false;
        }

        #region TaskFolder specific methods
        public List<TaskFolder> FindByNameStartsWith(string name)
        {
            throw new NotImplementedException();
        }

        public TaskFolder? FindOneByName(string name)
        {
            var result = Dao.FindByName(name);
            if (result == null)
                return null;

            return EntityFactory.ToModel(result);
        }

        public List<TaskFolder> FindEmpty()
        {
            throw new NotImplementedException();
        }

        public List<TaskFolder> FindNotEmpty()
        {
            throw new NotImplementedException();
        }

        public TaskFolder? FindByName(string name)
        {
            var result = Dao.FindByName(name);
            if (result == null)
                return null;

            return EntityFactory.ToModel(result);
        }

        public bool DeleteByName(string name)
        {
            var result = Dao.FindByName(name);
            if (result == null)
                return false;

            return Delete(result.Id);
        }

        #endregion
    }
}
