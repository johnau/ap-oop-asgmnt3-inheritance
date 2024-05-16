using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.Sqlite.Entity;

namespace TaskManagerCore.Infrastructure.Sqlite.Dao
{
    internal class TaskDataSqlDao
    {
        private readonly TaskFolderContext _context;

        public TaskDataSqlDao(TaskFolderContext context)
        {
            _context = context;
            Debug.WriteLine($"Database path: {_context.DbPath}.");
        }

        public bool Create(TaskDataEntity entity)
        {
            Debug.WriteLine($"Inserting a new TaskFolder Change entry for folder name: {entity.Description}");
            _context.Add(entity);
            _context.SaveChanges();

            return true;
        }

        public TaskDataEntity? FindById(string id)
        {
            Console.WriteLine("Querying for a folder name");
            var latestTaskState = _context.Tasks
                                            .Where(task => EF.Functions.Collate(task.Id, "NOCASE") == id)
                                            .OrderByDescending(j => j.TaskDataEntityId)
                                            .FirstOrDefault();

            return latestTaskState;
        }

        public List<TaskDataEntity> FindAll()
        {
            return _context.Tasks
                            .GroupBy(task => task.Id)  // Group tasks by ID
                            .Select(group => group
                                            .OrderByDescending(task => task.TaskDataEntityId)
                                            .First()) // For each group (ID), select the latest entry
                            .ToList();
        }

        public List<TaskDataEntity> GetHistory(string id)
        {
            return _context.Tasks
                            .Where(task => EF.Functions.Collate(task.Id, "NOCASE") == id)
                            .OrderByDescending(task => task.TaskDataEntityId)
                            .ToList();
        }

    }
}
