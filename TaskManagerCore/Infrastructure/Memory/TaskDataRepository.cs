using TaskManagerCore.Infrastructure.Memory.Dao;
using TaskManagerCore.Infrastructure.Memory.Entity;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.Infrastructure.Memory
{
    internal class TaskDataRepository : ITaskDataRepository
    {
        private readonly TaskDataDao Dao;
        private readonly IEntityFactory EntityFactory;

        public TaskDataRepository(TaskDataDao dao, IEntityFactory entityFactory)
        {
            Dao = dao;
            EntityFactory = entityFactory;
        }

        public List<TaskData> FindAll()
        {
            var all = Dao.FindAll();
            List<TaskData> tasks = new List<TaskData>();
            foreach (var task in all)
            {
                tasks.Add(EntityFactory.ToModel(task));
            }
            return tasks;
        }

        public List<TaskData> FindByIds(List<string> ids)
        {
            var matching = Dao.FindByIds(ids);
            List<TaskData> tasks = new List<TaskData>();
            foreach (var task in matching)
            {
                tasks.Add(EntityFactory.ToModel(task));
            }
            return tasks;
        }

        public TaskData? FindById(string id)
        {
            var one = Dao.FindById(id);
            if (one != null)
            {
                return EntityFactory.ToModel(one);
            }

            return null;
        }

        public string Save(TaskData o)
        {
            //return Dao.Save(TaskDataEntity.FromModel(o)); // Id gets generated here
            return Dao.Save(EntityFactory.FromModel(o)); // Id gets generated here
        }

        public bool Delete(string id)
        {
            return Dao.Delete(id);
        }
    }
}
