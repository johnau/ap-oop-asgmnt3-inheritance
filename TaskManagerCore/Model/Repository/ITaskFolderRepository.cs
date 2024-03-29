using TaskManagerCore.Configuration;

namespace TaskManagerCore.Model.Repository
{
    public interface ITaskFolderRepository : ICrudRepository<TaskFolder, string>
    {
        TaskFolder? FindByName(string name);
        List<TaskFolder> FindByNameStartsWith(string name);
        TaskFolder? FindOneByName(string name);
        List<TaskFolder> FindEmpty();
        List<TaskFolder> FindNotEmpty();

        bool DeleteByName(string name);
    }
}
