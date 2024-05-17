using TaskManagerCore.Infrastructure.Sqlite;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.Infrastructure
{
    internal class TaskDataDualRepositoryRunner : ITaskDataRepository
    {
        private ITaskDataRepository _taskDataRepository1;
        private ITaskDataRepository _taskDataRepository2;

        public TaskDataDualRepositoryRunner(ITaskDataRepository taskDataRepository1, ITaskDataRepository taskDataRepository2)
        {
            _taskDataRepository1 = taskDataRepository1;
            _taskDataRepository2 = taskDataRepository2;
        }

        public bool Delete(string id)
        {
            var result1 = _taskDataRepository1.Delete(id);
            var result2 = _taskDataRepository2.Delete(id);

            // compare results

            return result1;
        }

        public List<TaskData> FindAll()
        {
            var result1 = _taskDataRepository1.FindAll();
            var result2 = _taskDataRepository2.FindAll();

            // compare results

            return result1;
        }

        public List<TaskData> FindByCompleted(bool completed)
        {
            var result1 = _taskDataRepository1.FindByCompleted(completed);
            var result2 = _taskDataRepository2.FindByCompleted(completed);

            // compare results

            return result1;
        }

        public List<TaskData> FindByDescription(string description)
        {
            var result1 = _taskDataRepository1.FindByDescription(description);
            var result2 = _taskDataRepository2.FindByDescription(description);

            // compare results

            return result1;
        }

        public List<TaskData> FindByDueDate(DateTime dueDate)
        {
            var result1 = _taskDataRepository1.FindByDueDate(dueDate);
            var result2 = _taskDataRepository2.FindByDueDate(dueDate);

            // compare results

            return result1;
        }

        public TaskData? FindById(string id)
        {
            var result1 = _taskDataRepository1.FindById(id);
            var result2 = _taskDataRepository2.FindById(id);

            // compare results

            return result1;
        }

        public List<TaskData> FindByIds(List<string> ids)
        {
            var result1 = _taskDataRepository1.FindByIds(ids);
            var result2 = _taskDataRepository2.FindByIds(ids);

            // compare results

            return result1;
        }

        public List<TaskData> FindByNotes(string notes)
        {
            var result1 = _taskDataRepository1.FindByNotes(notes);
            var result2 = _taskDataRepository2.FindByNotes(notes);

            // compare results

            return result1;
        }

        public string Save(TaskData o)
        {
            var result1 = _taskDataRepository1.Save(o);
            var result2 = _taskDataRepository2.Save(EntityFactory.WithId(o, result1));

            // compare results

            return result1;
        }
    }
}
