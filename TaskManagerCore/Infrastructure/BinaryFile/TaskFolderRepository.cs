using TaskManagerCore.Model;
using TaskManagerCore.Model.Repository;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    internal class TaskFolderRepository : ITaskFolderRepository
    {
        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        public List<TaskFolder> FindAll()
        {
            throw new NotImplementedException();
        }

        public TaskFolder? FindById(string id)
        {
            throw new NotImplementedException();
        }

        public List<TaskFolder> FindByIds(List<string> ids)
        {
            throw new NotImplementedException();
        }

        public string Save(TaskFolder o)
        {
            throw new NotImplementedException();
        }
    }
}
