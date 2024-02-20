using TaskManagerCore.Infrastructure.Memory.Dao;
using TaskManagerCore.Infrastructure.Memory.Entity;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.Infrastructure.Memory
{
    internal class TaskFolderRepository : ITaskFolderRepository
    {
        private readonly TaskFolderDao Dao;
        private readonly IEntityFactory EntityFactory;

        internal TaskFolderRepository(TaskFolderDao dao, IEntityFactory entityFactory)
        {
            Dao = dao;
            EntityFactory = entityFactory;
        }

        public List<TaskFolder> FindAll()
        {
            var all = Dao.FindAll();
            List<TaskFolder> folders = new List<TaskFolder>();
            foreach (var folder in all)
            {
                folders.Add(EntityFactory.ToModel(folder));
            }
            return folders;
        }

        public List<TaskFolder> FindByIds(List<string> ids)
        {
            var matching = Dao.FindByIds(ids);
            List<TaskFolder> folders = new List<TaskFolder>();
            foreach (var folder in matching)
            {
                folders.Add(EntityFactory.ToModel(folder));
            }
            return folders;
        }

        public TaskFolder? FindById(string id)
        {
            var one = Dao.FindById(id);
            if (one != null)
            {
                return EntityFactory.ToModel(one); 
            }

            return null;
        }

        public string Save(TaskFolder o)
        {
            return Dao.Save(EntityFactory.FromModel(o));
        }

        public bool Delete(string id)
        {
            return Dao.Delete(id);
        }
    }
}
