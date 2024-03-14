using TaskManagerCore.Configuration;

namespace TaskManagerCore.Model.Repository
{
    public interface ITaskFolderRepository : ICrudRepository<TaskFolder, string>
    {
        List<TaskFolder> FindByName(string name);
        TaskFolder? FindOneByName(string name);
        List<TaskFolder> FindEmpty();
        List<TaskFolder> FindNotEmpty();
    }
}
