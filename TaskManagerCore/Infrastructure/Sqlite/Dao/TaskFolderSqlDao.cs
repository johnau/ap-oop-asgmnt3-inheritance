using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Diagnostics;
using System.Linq;
using TaskManagerCore.Infrastructure.Sqlite.Entity;

namespace TaskManagerCore.Infrastructure.Sqlite.Dao
{
    internal class TaskFolderSqlDao
    {
        private readonly SqliteContext _context;

        public TaskFolderSqlDao(SqliteContext context)
        {
            _context = context;
            Debug.WriteLine($"Database path: {_context.DbPath}.");
        }

        public bool Save(TaskFolderEntityV2 entity)
        {
            Debug.WriteLine($"Creating or Updating Task: {entity.Name}");

            var existing = _context.Folders
                                    .FirstOrDefault(task => task.GlobalId == entity.GlobalId);

            var existingWithSameName = _context.Folders
                        .FirstOrDefault(task => task.Name == entity.Name);

            if (existing == null && existingWithSameName != null)
                throw new InvalidDataException($"There is already a folder called: {entity.Name.ToUpper()}");

            if (existing != null)
            {
                existing.Name = entity.Name;
                existing.TaskIds = entity.TaskIds;
                //existing.Tasks = entity.Tasks;

                // Populating the relationship here out of laziness, performance will not be great
                // rebuilding the relationships every save.
                foreach (var taskId in entity.TaskIds)
                {
                    var task = _context.Tasks.FirstOrDefault(task => task.GlobalId == taskId);
                    if (task == null)
                    {
                        Debug.WriteLine($"SQL Database Sync Error: Task does not exist: {taskId}");
                        continue;
                    }

                    existing.Tasks.Add(task);
                }

                _context.Update(existing);
            }
            else
            {
                _context.Add(entity);
            }

            _context.SaveChanges();

            return true;
        }

        public TaskFolderEntityV2? FindById(string id)
        {
            Console.WriteLine("Querying for a folder id");
            var latestFolderState = _context.Folders
                                            .Where(folder => EF.Functions.Collate(folder.GlobalId, "BINARY") == id)
                                            .FirstOrDefault();

            return latestFolderState;
        }

        public List<TaskFolderEntityV2> FindByIds(List<string> ids)
        {
            return _context.Folders
                            .Where(folder => ids.Contains(folder.GlobalId))
                            .ToList();
        }

        public TaskFolderEntityV2? FindByName(string folderName)
        {
            Console.WriteLine("Querying for a folder name");
            var latestFolderState = _context.Folders
                                            .Where(folder => EF.Functions.Collate(folder.Name, "NOCASE") == folderName)
                                            .FirstOrDefault();

            return latestFolderState;
        }

        /// <summary>
        /// Only gets the latest 100 folders
        /// </summary>
        /// <returns></returns>
        public List<TaskFolderEntityV2> FindAll()
        {
            return _context.Folders
                                .OrderByDescending(folder => folder.Id)
                                .Take(100)
                                .ToList();
        }

        public bool Delete(string globalId)
        {
            try
            {
                var entity = _context.Folders.FirstOrDefault(folder => folder.GlobalId == globalId);
                if (entity == null)
                    return false;

                _context.Folders.Remove(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public List<TaskFolderEntityV2> FindByNameStartsWith(string name)
        {
            string lowerName = name.ToLower();

            return _context.Folders
                           .Where(folder => EF.Functions.Like(folder.Name.ToLower(), $"{lowerName}%"))
                           .ToList();
        }

        public List<TaskFolderEntityV2> FindEmpty()
        {
            return _context.Folders
                           .Where(folder => folder.TaskIds == null || folder.TaskIds.Count == 0)
                           .ToList();
        }

        public List<TaskFolderEntityV2> FindNotEmpty()
        {
            return _context.Folders
               .Where(folder => folder.TaskIds != null && folder.TaskIds.Count > 0)
               .ToList();
        }

    }
}
