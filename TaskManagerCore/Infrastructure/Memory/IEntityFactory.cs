using TaskManagerCore.Infrastructure.Memory.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.Memory
{
    public interface IEntityFactory
    {
        public TaskFolderEntity FromModel(TaskFolder taskFolder);

        public TaskDataEntity FromModel(TaskData taskData);

        //public RepeatingTaskDataEntity FromModel(RepeatingTaskData taskData);

        //public HabitualTaskDataEntity FromModel(HabitualTaskData taskData);

        public TaskFolder ToModel(TaskFolderEntity taskFolder);

        public TaskData ToModel(TaskDataEntity taskData);

        //public RepeatingTaskData ToModel(RepeatingTaskDataEntity repeatingTaskData);

        //public HabitualTaskData ToModel(HabitualTaskDataEntity habitualTaskData);
    }
}
