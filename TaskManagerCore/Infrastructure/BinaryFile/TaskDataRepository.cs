using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    internal class TaskDataRepository : ITaskDataRepository
    {
        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public List<TaskData> FindAll()
        {
            throw new NotImplementedException();
        }

        public TaskData? FindById(string id)
        {
            throw new NotImplementedException();
        }

        public List<TaskData> FindByIds(List<string> ids)
        {
            throw new NotImplementedException();
        }

        public string Save(TaskData o)
        {
            throw new NotImplementedException();
        }
    }
}
