using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.Sqlite.Entity;

namespace TaskManagerCore.Infrastructure.Sqlite.Dao
{
    internal class TaskFolderSqlDao
    {
        private readonly TaskFolderContext _context;

        public TaskFolderSqlDao(TaskFolderContext context)
        {
            _context = context;
            Debug.WriteLine($"Database path: {_context.DbPath}.");
        }

        public bool Create(TaskFolderEntity entity)
        {
            Debug.WriteLine($"Inserting a new TaskFolder Change entry for folder name: {entity.Name}");
            _context.Add(entity);
            _context.SaveChanges();

            return true;
        }

        public TaskFolderEntity? FindById(string id)
        {
            Console.WriteLine("Querying for a folder id");
            var latestFolderState = _context.Folders
                                            .Where(folder => EF.Functions.Collate(folder.Id, "BIN") == id)
                                            .OrderByDescending(j => j.TaskFolderEntityId)
                                            .FirstOrDefault();

            return latestFolderState;
        }

        public List<TaskFolderEntity> FindByIds(List<string> ids)
        {
            var matches = new List<TaskFolderEntity>();

            foreach (var id in ids)
            {
                var result = FindById(id);
                if (result != null)
                    matches.Add(result);
            }

            return matches;
        }

        public TaskFolderEntity? FindByName(string folderName)
        {
            Console.WriteLine("Querying for a folder name");
            var latestFolderState = _context.Folders
                                            .Where(folder => EF.Functions.Collate(folder.Name, "NOCASE") == folderName)
                                            .OrderByDescending(j => j.TaskFolderEntityId)
                                            .FirstOrDefault();

            return latestFolderState;
        }

        public List<TaskFolderEntity> FindAll()
        {
            return _context.Folders
                            .GroupBy(folder => folder.Id)  // Group tasks by ID
                            .Select(group => group
                                            .OrderByDescending(folder => folder.TaskFolderEntityId)
                                            .First()) // For each group (ID), select the latest entry
                            .ToList();
        }

        public List<TaskFolderEntity> GetHistory(string id)
        {
            return _context.Folders
                            .Where(j => EF.Functions.Collate(j.Name, "NOCASE") == id)
                            .OrderByDescending(j => j.TaskFolderEntityId)
                            .ToList();
        }
    }
}
