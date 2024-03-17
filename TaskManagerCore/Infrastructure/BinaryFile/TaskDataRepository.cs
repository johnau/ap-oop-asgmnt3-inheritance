using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    internal class TaskDataRepository : ITaskDataRepository
    {
        readonly TaskDataDao Dao;
        public TaskDataRepository(TaskDataDao dao)
        {
            Dao = dao;
        }

        #region ICrudRepository methods
        public List<TaskData> FindAll()
        {
            var all = Dao.FindAll();
            return EntityFactory.ToModel(all);
        }

        public List<TaskData> FindByIds(List<string> ids)
        {
            var matching = Dao.FindByIds(ids);
            return EntityFactory.ToModel(matching);
        }

        public TaskData? FindById(string id)
        {
            var one = Dao.FindById(id);
            if (one == null) return null;
            
            return EntityFactory.ToModel(one);
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
        #endregion

        #region TaskData specific methods

        public List<TaskData> FindByDueDate(DateTime dueDate)
        {
            var result = Dao.FindByDueDate(dueDate);
            return EntityFactory.ToModel(result);
        }

        public List<TaskData> FindByDescription(string description)
        {
            var result = Dao.FindByDescription(description);
            return EntityFactory.ToModel(result);
        }

        //public TaskData? FindOneByDescription(string description)
        //{
        //    var result = Dao.FindOneByDescription(description);
        //    if (result == null) return null;
        //    return EntityFactory.ToModel(result);
        //}

        public List<TaskData> FindByNotes(string notes)
        {
            var result = Dao.FindByNotes(notes);
            return EntityFactory.ToModel(result);
        }

        public List<TaskData> FindByCompleted(bool completed)
        {
            var result = Dao.FindByCompleted(completed);
            return EntityFactory.ToModel(result);
        }

        #endregion
    }
}
