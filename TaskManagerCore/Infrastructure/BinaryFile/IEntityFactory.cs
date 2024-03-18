using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    internal interface IEntityFactory
    {
        TaskFolderEntity FromModel(TaskFolder taskFolder);
        TaskDataEntity FromModel(TaskData taskData);
        //TaskDataEntity TaskFromValues(string className, string id, string description, string notes, bool completed, DateTime? dueDate, TimeInterval? interval = null, int? repetitions = null, int? streak = null)
        TaskFolder ToModel(TaskFolderEntity taskFolder);
        TaskData ToModel(TaskDataEntity taskData);
    }
}