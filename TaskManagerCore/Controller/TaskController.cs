﻿using System.Diagnostics;
using System.Text.RegularExpressions;
using TaskManagerCore.Configuration;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Dto;
using TaskManagerCore.Model.Dto.Mapper;

namespace TaskManagerCore.Controller
{
    public partial class TaskController
    {
        [GeneratedRegex(@"^due[_\.]?date$")]
        private static partial Regex Regex_DueDatePropertyNameLowerCase();

        private readonly ICrudRepository<TaskData, string> TaskDataRepository;
        private readonly ICrudRepository<TaskFolder, string> TaskFolderRepository;
        private readonly GetTaskDtoMapper DtoMapperGetTask;
        private readonly GetFolderDtoMapper DtoMapperGetFolder;
        private readonly CreateTaskDtoMapper DtoMapperCreateTask;
        private readonly CreateFolderDtoMapper DtoMapperCreateFolder;

        public TaskController(ICrudRepository<TaskData, string> taskDataRepository,
                                ICrudRepository<TaskFolder, string> taskFolderRepository)
        {
            TaskDataRepository = taskDataRepository;
            TaskFolderRepository = taskFolderRepository;
            DtoMapperGetTask = new GetTaskDtoMapper();
            DtoMapperGetFolder = new GetFolderDtoMapper();
            DtoMapperCreateTask = new CreateTaskDtoMapper();
            DtoMapperCreateFolder = new CreateFolderDtoMapper();
        }

        /// <summary>
        /// Get All Tasks
        /// </summary>
        /// <returns></returns>
        public List<GetTaskDto> GetTasks()
        {
            var all = TaskDataRepository.FindAll();
            List<GetTaskDto> dtos = new List<GetTaskDto>();

            foreach (var item in all)
            {
                dtos.Add(DtoMapperGetTask.Map(item));
            }

            return dtos;
        }

        /// <summary>
        /// Get a Task (with the Task Id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GetTaskDto? GetTaskById(string id)
        {
            var task = TaskDataRepository.FindById(id);
            if (task == null)
            {
                return null;
            }

            return DtoMapperGetTask.Map(task);
        }

        /// <summary>
        /// Get List of tasks by id's
        /// Use to get task data from list in folder
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<GetTaskDto> GetTasksByIds(List<string> ids)
        {
            var tasks = TaskDataRepository.FindByIds(ids);

            var dtos = new List<GetTaskDto>();
            foreach (var task in tasks)
            {
                dtos.Add(DtoMapperGetTask.Map(task));
            }
            return dtos;
        }

        /// <summary>
        /// Get All Task Folders
        /// </summary>
        /// <returns></returns>
        public List<GetFolderDto> GetTaskFolders()
        {
            var all = TaskFolderRepository.FindAll();
            List<GetFolderDto> dtos = new List<GetFolderDto>();

            foreach (var item in all)
            {
                dtos.Add(DtoMapperGetFolder.Map(item));
            }

            return dtos;
        }

        /// <summary>
        /// Get a Task Folder (with the TaskFolder Id)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GetFolderDto? GetTaskFolderById(string id)
        {
            var folder = TaskFolderRepository.FindById(id);
            if (folder == null)
            {
                return null;
            }

            return DtoMapperGetFolder.Map(folder);
        }

        /// <summary>
        /// This method is spec'd in the TaskFolder class however it doesn't seem
        /// unreasonable to shift it up a few levels to avoid the reference between
        /// the two entities.
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long CountIncomplete(string folderId)
        {
            var folder = TaskFolderRepository.FindById(folderId);
            if (folder == null)
                throw new Exception($"Unable to find Folder with Id: {folderId}");

            var idsInFolder = folder.TaskIds;
            var tasksInFolder = TaskDataRepository.FindByIds(idsInFolder);

            var incomplete = 0L;
            foreach (var task in tasksInFolder)
            {
                if (!task.Completed) incomplete++;
            }

            return incomplete;
        }

        /// <summary>
        /// Sets a task to Complete = true by default.  Can be used to set completed to false
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool CompleteTask(string id, bool completed = true)
        {
            var task = TaskDataRepository.FindById(id);
            if (task == null)
                throw new Exception($"Unable to find Task with Id: {id}");

            var savedId = TaskDataRepository.Save(task.WithCompleted(completed));

            //Debug.WriteLine($"Updated Task '{id}' => Completed=True");
            return savedId != null; // not good, improve how this response is calculated
        }

        /// <summary>
        /// Deletes a task, given folder Id and task Id, assumes that caller knows
        /// the folder Id, since they will be viewing tasks through the folders
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool DeleteTaskFromFolder(string folderId, string taskId)
        {

            var folder = TaskFolderRepository.FindById(folderId);
            if (folder == null)
            {
                Debug.WriteLine($"Unable to find Folder with Id: {folderId}");
                throw new Exception($"Unable to find Folder with Id: {folderId}");
            }

            var task = TaskDataRepository.FindById(taskId);
            if (task == null)
            {
                Debug.WriteLine($"Unable to find Task with Id: {taskId}");
                throw new Exception($"Unable to find Task with Id: {taskId}");
            }

            var savedFolderId = TaskFolderRepository.Save(folder.WithoutTask(taskId));
            var didDeleteTask = TaskDataRepository.Delete(taskId);

            if (savedFolderId != null && didDeleteTask) // not great, but fits purpose
                return true;

            return false;
        }

        /// <summary>
        /// Create a new Task in a specified TaskFolder (with the TaskFolder Id)
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="folderId"></param>
        /// <exception cref="Exception"></exception>
        public string CreateTask(CreateTaskDto dto)
        {
            var folder = TaskFolderRepository.FindById(dto.InFolderId);
            if (folder == null)
                throw new Exception($"Could not find Folder: {dto.InFolderId}");

            
            var task = DtoMapperCreateTask.Map(dto);
            var taskId = TaskDataRepository.Save(task);

            var folderUpdated = folder.WithTask(taskId);
            var updatedFolderId = TaskFolderRepository.Save(folderUpdated);
            if (taskId == null || updatedFolderId == null)
                throw new Exception($"Unable to Create task in folder {dto.InFolderId}");

            return taskId;
        }

        /// <summary>
        /// Create a new TaskFolder
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string CreateTaskFolder(CreateFolderDto dto)
        {
            var taskFolder = DtoMapperCreateFolder.Map(dto);
            try
            {
                return TaskFolderRepository.Save(taskFolder);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not save Task Folder: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a folder (must be empty)
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        public bool DeleteTaskFolder(string folderId)
        {
            return TaskFolderRepository.Delete(folderId);
        }

        /// <summary>
        /// Move a Task from one TaskFolder to another (with Task Id, and TaskFolder Id's)
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="folderIdFrom"></param>
        /// <param name="folderIdTo"></param>
        /// <exception cref="Exception"></exception>
        public bool MoveTask(string taskId, string folderIdFrom, string folderIdTo)
        {
            var task = TaskDataRepository.FindById(taskId);
            if (task == null)
                throw new Exception($"Could not find Task with Id: {taskId}");

            var fromFolder = TaskFolderRepository.FindById(folderIdFrom);
            var toFolder = TaskFolderRepository.FindById(folderIdTo);
            if (fromFolder == null || toFolder == null)
                throw new Exception($"Could not find target Folders for move ({folderIdFrom}, {folderIdTo})");

            fromFolder = fromFolder.WithoutTask(taskId);
            toFolder = toFolder.WithTask(taskId);

            var savedTaskId = TaskFolderRepository.Save(fromFolder);
            var savedFromFolderId = TaskFolderRepository.Save(fromFolder);
            var savedToFolderId = TaskFolderRepository.Save(toFolder);

            if (savedTaskId == null || savedFromFolderId == null || savedToFolderId == null)
                return false;

            return true;
        }

        /// <summary>
        /// Update modifiable properties Task
        /// </summary>
        /// <param name="id"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string UpdateTaskProperty(string id, string property, object value)
        {
            property = property.ToLower();
            var task = TaskDataRepository.FindById(id);
            if (task == null)
                throw new Exception($"Task not found with id: {id}");

            if (property == "description")
                task = task.WithDescription((string)value);
            else if (property == "notes")
                task = task.WithNotes((string)value);
            else if (Regex_DueDatePropertyNameLowerCase().IsMatch(property)) // due Date
                task = task.WithDueDate(new DateTime((long)value));
            else
                throw new Exception($"Unrecognized property: {property}");

            try
            {
                return TaskDataRepository.Save(task);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not save Task: {ex.Message}");
            }
        }

        /// <summary>
        /// Update modifiable properties Folder
        /// </summary>
        /// <param name="id"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string UpdateFolderProperty(string id, string property, object value)
        {
            property = property.ToLower();
            var folder = TaskFolderRepository.FindById(id);
            if (folder == null)
                throw new Exception($"Folder not found with id: {id}");

            if (property == "name")
                folder = folder.WithName((string)value);

            try
            {
                return TaskFolderRepository.Save(folder);
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not save Folder: {ex.Message}");
            }
        }

    }
}
