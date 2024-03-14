
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.BinaryFile
{
    // Too lazy to do this properly right now
    internal interface IEntityFactory
    {
        public TaskFolderEntity FromModel(TaskFolder taskFolder);
        public List<TaskFolderEntity> FromModel(List<TaskFolder> taskFolders);
        public TaskDataEntity FromModel(TaskData taskData);
        public List<TaskDataEntity> FromModel(List<TaskData> taskData);

        public TaskFolder ToModel(TaskFolderEntity taskFolder);
        public List<TaskFolder> ToModel(List<TaskFolderEntity> taskFolders);
        public TaskData ToModel(TaskDataEntity taskData);
        public List<TaskData> ToModel(List<TaskDataEntity> taskDatas);

        //public RepeatingTaskDataEntity FromModel(RepeatingTaskData taskData);

        //public HabitualTaskDataEntity FromModel(HabitualTaskData taskData);


        //public RepeatingTaskData ToModel(RepeatingTaskDataEntity repeatingTaskData);

        //public HabitualTaskData ToModel(HabitualTaskDataEntity habitualTaskData);
    }
}
