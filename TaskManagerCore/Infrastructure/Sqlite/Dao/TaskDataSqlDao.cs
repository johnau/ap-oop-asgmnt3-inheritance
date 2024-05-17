using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.Sqlite.Entity;

namespace TaskManagerCore.Infrastructure.Sqlite.Dao
{
    internal class TaskDataSqlDao
    {
        private readonly SqliteContext _context;

        public TaskDataSqlDao(SqliteContext context)
        {
            _context = context;
            Debug.WriteLine($"Database path: {_context.DbPath}.");
        }

        public bool Save(TaskDataEntityV2 entity)
        {
            Debug.WriteLine($"Creating or Updating Task: {entity.Description}");

            var existing = _context.Tasks
                                    .FirstOrDefault(task => task.GlobalId == entity.GlobalId);

            if (existing != null)
            {
                existing.Description = entity.Description;
                existing.Notes = entity.Notes;
                existing.Completed = entity.Completed;
                existing.DueDate = entity.DueDate;
                existing.RepeatingInterval = entity.RepeatingInterval;
                existing.Repetitions = entity.Repetitions;
                existing.Streak = entity.Streak;

                _context.Update(existing);
            }
            else
            {
                _context.Add(entity);
            }

            _context.SaveChanges();

            return true;
        }

        public TaskDataEntityV2? FindById(string id)
        {
            Debug.WriteLine($"Querying for a folder by id: {id}");
            var latestTaskState = _context.Tasks
                                            .Where(task => EF.Functions.Collate(task.GlobalId, "NOCASE") == id)
                                            .FirstOrDefault();

            return latestTaskState;
        }

        public List<TaskDataEntityV2> FindAll()
        {
            Debug.WriteLine($"Finding all tasks");
            return _context.Tasks
                           .Take(100)
                           .ToList();
        }

        public bool Delete(string globalId)
        {
            Debug.WriteLine($"Deleting task: {globalId}");
            try
            {
                var entity = _context.Tasks.FirstOrDefault(folder => folder.GlobalId == globalId);
                if (entity == null)
                    return false;

                _context.Tasks.Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<TaskDataEntityV2> FindByDueDate(DateTime dueDate)
        {
            Debug.WriteLine($"Finding by DueDate {dueDate.Date}");
            return _context.Tasks
                           .Where(task => task.DueDate.HasValue && task.DueDate.Value.Date == dueDate.Date)
                           .ToList();
        }

        public List<TaskDataEntityV2> FindByDescription(string description)
        {
            Debug.WriteLine($"Querying for tasks with description: {description}");
            var tasksWithDescription = _context.Tasks
                                               .Where(task => EF.Functions.Collate(task.Description, "NOCASE") == description)
                                               .ToList();

            return tasksWithDescription;
        }

        public List<TaskDataEntityV2> FindByNotes(string notes)
        {
            Debug.WriteLine($"Querying for tasks with notes: {notes}");
            var tasksWithDescription = _context.Tasks
                                               .Where(task => EF.Functions.Collate(task.Notes, "NOCASE") == notes)
                                               .ToList();

            return tasksWithDescription;
        }

        public List<TaskDataEntityV2> FindByCompleted(bool completed)
        {
            Debug.WriteLine($"Querying for tasks with complete status: {completed}");
            var tasksByCompletion = _context.Tasks
                                            .Where(task => task.Completed == completed)
                                            .ToList();

            return tasksByCompletion;
        }

    }
}
