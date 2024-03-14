using TaskManagerCore.Configuration;

namespace TaskManagerCore.Model.Repository
{
    public interface ITaskDataRepository : ICrudRepository<TaskData, string>
    {
        List<TaskData> FindByDueDate(DateTime dueDate);
        List<TaskData> FindByDescription(string description);
        // Match unique description exactly
        //TaskData? FindOneByDescription(string description);
        List<TaskData> FindByNotes(string notes);
        List<TaskData> FindByCompleted(bool completed);
        //List<RepeatingTaskData> FindByInterval(int interval);
        //List<HabitualTaskData> FindByHasStreak(bool hasStreak);
    }
}
