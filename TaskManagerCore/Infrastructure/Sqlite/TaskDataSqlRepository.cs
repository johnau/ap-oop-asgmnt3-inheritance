using TaskManagerCore.Model.Repository;
using TaskManagerCore.Model;
using TaskManagerCore.Infrastructure.Sqlite.Dao;
using Microsoft.VisualBasic;

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
            if (Dao.Save(EntityFactory.FromModel(o)))
            {
                return o.Id;
            }

            throw new Exception("Unable to save to database");
        }

        public bool Delete(string id)
        {
            return Dao.Delete(id);
        }
        #endregion

        #region TaskData specific methods

        public List<TaskData> FindByDueDate(DateTime dueDate)
        {
            var results = Dao.FindByDueDate(dueDate);

            return EntityFactory.ToModel(results);
        }

        public List<TaskData> FindByDescription(string description)
        {
            var results = Dao.FindByDescription(description);

            return EntityFactory.ToModel(results);
        }

        //public TaskData? FindOneByDescription(string description)
        //{
        //    var result = Dao.FindOneByDescription(description);
        //    if (result == null) return null;
        //    return EntityFactory.ToModel(result);
        //}

        public List<TaskData> FindByNotes(string notes)
        {
            var results = Dao.FindByNotes(notes);

            return EntityFactory.ToModel(results);
        }

        public List<TaskData> FindByCompleted(bool completed)
        {
            var results = Dao.FindByCompleted(completed);

            return EntityFactory.ToModel(results);
        }

        #endregion
    }
}
