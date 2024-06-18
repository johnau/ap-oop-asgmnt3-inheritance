using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TaskManagerCore.Configuration;
using TaskManagerCore.SQL.Sqlite.Entity;

namespace TaskManagerCore.SQL.Sqlite.Dao
{
    public class TaskFolderSqlDao : ICrudRepository<TaskFolderEntityV2, string>
    {
        private readonly SqliteContext _context;

        public TaskFolderSqlDao(SqliteContext context)
        {
            _context = context;
            Debug.WriteLine($"Database path: {_context.DbPath}.");
        }

        public TaskFolderEntityV2 Save(TaskFolderEntityV2 entity)
        {
            Debug.WriteLine($"Creating or Updating Folder: {entity.Name}");

            var existing = _context.Folders
                                    .Where(folder => folder.GlobalId == entity.GlobalId)
                                    .FirstOrDefault();

            var existingWithSameName = _context.Folders
                                                .FirstOrDefault(folder => folder.Name.ToLower() == entity.Name.ToLower());

            if (existing == null && existingWithSameName != null)
                throw new InvalidDataException($"There is already a folder called: {entity.Name.ToUpper()}");

            TaskFolderEntityV2 saved;
            if (existing != null)
            {
                existing.Name = entity.Name;
                foreach (var taskId in entity.TaskIds)
                {
                    if (!existing.TaskIds.Contains(taskId))
                        existing.TaskIds.Remove(taskId);
                }

                // existing.Tasks = entity.Tasks;

                // Populating the relationship here out of laziness, performance will not be great
                // rebuilding the relationships every save.

                existing.Tasks.Clear();

                foreach (var taskId in entity.TaskIds)
                {
                    var task = _context.Tasks
                                        .Where(t => t.GlobalId == taskId)
                                        .FirstOrDefault();

                    if (task == null)
                    {
                        Debug.WriteLine($"SQL Database Sync Error: Task does not exist: {taskId}");
                        continue;
                    }

                    existing.Tasks.Add(task);
                }

                _context.Update(existing);
                saved = existing;
            }
            else
            {
                _context.Add(entity);
                saved = entity;
            }

            _context.SaveChanges();

            return saved;
        }

        public TaskFolderEntityV2 FindById(string id)
        {
            Console.WriteLine("Querying for a folder id");
            var latestFolderState = _context.Folders
                                            .Include(folder => folder.Tasks)
                                            //.Where(folder => EF.Functions.Collate(folder.GlobalId, "BINARY") == id)
                                            .Where(folder => folder.GlobalId == id)
                                            //.FromSqlRaw("SELECT * FROM Tasks WHERE GlobalId COLLATE BINARY = {0} LIMIT 1", id) // adding a few SQL statements in - incase that's required for the curriculum
                                            .FirstOrDefault();

            return latestFolderState;
        }

        public List<TaskFolderEntityV2> FindByIds(List<string> ids)
        {
            return _context.Folders
                            .Include(folder => folder.Tasks)
                            .Where(folder => ids.Contains(folder.GlobalId))
                            .ToList();
        }

        public TaskFolderEntityV2 FindByName(string folderName)
        {
            Console.WriteLine("Querying for a folder name");
            var latestFolderState = _context.Folders
                                            .Include(folder => folder.Tasks)
                                            .Where(folder => folder.Name.ToLower() == folderName.ToLower())
                                            //.FromSqlRaw("SELECT * FROM Tasks WHERE GlobalId COLLATE NOCASE = {0} LIMIT 1", folderName)
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
                            .Include(folder => folder.Tasks)
                            .OrderByDescending(folder => folder.Id)
                            .Take(100)
                            .ToList();
        }

        public bool Delete(string globalId)
        {
            try
            {
                var entity = _context.Folders
                                        .Include(folder => folder.Tasks)
                                        .FirstOrDefault(folder => folder.GlobalId == globalId);
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
                            .Include(folder => folder.Tasks)
                            .Where(folder => EF.Functions.Like(folder.Name.ToLower(), $"{lowerName}%"))
                            .ToList();
        }

        public List<TaskFolderEntityV2> FindEmpty()
        {
            return _context.Folders
                            .Include(folder => folder.Tasks)
                            .Where(folder => folder.TaskIds == null || folder.TaskIds.Count() == 0)
                            .ToList();
        }

        public List<TaskFolderEntityV2> FindNotEmpty()
        {
            return _context.Folders
                            .Include(folder => folder.Tasks)
                            .Where(folder => folder.TaskIds != null && folder.TaskIds.Count() > 0)
                            .ToList();
        }

    }
}
