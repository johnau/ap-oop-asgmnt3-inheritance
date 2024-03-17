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

        public TaskFolder? FindById(string id)
        {
            var one = Dao.FindById(id);
            if (one == null) return null;
            
            return EntityFactory.ToModel(one);
        }

        public string Save(TaskFolder o)
        {
            return Dao.Save(EntityFactory.FromModel(o));
        }

        public bool Delete(string id)
        {
            return Dao.Delete(id);
        }

        #region TaskFolder specific methods
        public List<TaskFolder> FindByName(string name)
        {
            var results = Dao.FindByName(name);
            return EntityFactory.ToModel(results);
        }

        public TaskFolder? FindOneByName(string name)
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

        #endregion
    }
}
