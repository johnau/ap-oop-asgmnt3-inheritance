using TaskManagerCore.Model.Repository;
using TaskManagerCore.Model;
using TaskManagerCore.Infrastructure.Sqlite.Dao;

namespace TaskManagerCore.Infrastructure.Sqlite
{
    internal class TaskDataSqlRepository : ITaskDataRepository
    {
        readonly TaskDataSqlDao Dao;
        public TaskDataSqlRepository(TaskDataSqlDao dao)
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
            var matched = new List<TaskData>();
            foreach (var id in ids)
            {
                var match = FindById(id);
                if (match != null) 
                    matched.Add(match);
            }

            return matched;
        }

        public TaskData? FindById(string id)
        {
            var one = Dao.FindById(id);
            if (one == null) return null;

            return EntityFactory.ToModel(one);
        }

        public string Save(TaskData o)
        {
            if (Dao.Create(EntityFactory.FromModel(o)))
            {
                return o.Id;
            }

            throw new Exception("Unable to save to database");
        }

        public bool Delete(string id)
        {
            var existing = FindById(id);
            if (existing != null)
            {
                var deleting = existing.WithDescription("<DELETED>")
                        .WithNotes("<DELETED>")
                        .WithCompleted(false)
                        .WithDueDate(DateTime.MinValue);
                Save(deleting);
                return true;
            }

            return false;
        }
        #endregion

        #region TaskData specific methods

        public List<TaskData> FindByDueDate(DateTime dueDate)
        {
            throw new NotImplementedException();
        }

        public List<TaskData> FindByDescription(string description)
        {
            throw new NotImplementedException();
        }

        //public TaskData? FindOneByDescription(string description)
        //{
        //    var result = Dao.FindOneByDescription(description);
        //    if (result == null) return null;
        //    return EntityFactory.ToModel(result);
        //}

        public List<TaskData> FindByNotes(string notes)
        {
            throw new NotImplementedException();
        }

        public List<TaskData> FindByCompleted(bool completed)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
