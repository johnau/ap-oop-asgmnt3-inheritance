using System;
using System.Collections.Generic;
using TaskManagerCore.Configuration;

namespace TaskManagerCore.Model.Repository
{
    /// <summary>
    /// Interface for the repository managing task data entities.
    /// </summary>
    public interface ITaskDataRepository : ICrudRepository<TaskData, string>
    {
        /// <summary>
        /// Finds tasks by due date.
        /// </summary>
        /// <param name="dueDate">The due date to search for.</param>
        /// <returns>A list of task data entities matching the due date.</returns>
        List<TaskData> FindByDueDate(DateTime dueDate);

        /// <summary>
        /// Finds tasks by description.
        /// </summary>
        /// <param name="description">The description to search for.</param>
        /// <returns>A list of task data entities matching the description.</returns>
        List<TaskData> FindByDescription(string description);

        /// <summary>
        /// Finds tasks by notes.
        /// </summary>
        /// <param name="notes">The notes to search for.</param>
        /// <returns>A list of task data entities matching the notes.</returns>
        List<TaskData> FindByNotes(string notes);

        /// <summary>
        /// Finds tasks by completion status.
        /// </summary>
        /// <param name="completed">The completion status to search for.</param>
        /// <returns>A list of task data entities matching the completion status.</returns>
        List<TaskData> FindByCompleted(bool completed);

        // Uncomment and implement the following methods if needed
        //List<RepeatingTaskData> FindByInterval(int interval);
        //List<HabitualTaskData> FindByHasStreak(bool hasStreak);
    }
}