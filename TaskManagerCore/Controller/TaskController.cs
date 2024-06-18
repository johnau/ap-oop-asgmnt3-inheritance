using NaturalLanguageProcessor;
using NaturalLanguageProcessor.Aggregates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Dto;
using TaskManagerCore.Model.Dto.Mapper;
using TaskManagerCore.Model.Repository;
using TaskManagerCore.Utility;

namespace TaskManagerCore.Controller
{
    /// <summary>
    /// The TaskController class provides an API to all functionality of the TaskManager-Core system.
    /// </summary>
    /// <include file="TaskControllerDocs.xml" path="doc/members/member[@name='M:TaskManagerCore.Controller.TaskController']/*" />
    public partial class TaskController
    {
        private readonly ITaskDataRepository TaskDataRepository;
        private readonly ITaskFolderRepository TaskFolderRepository;
        private readonly GetTaskDtoMapper DtoMapperGetTask;
        private readonly GetFolderDtoMapper DtoMapperGetFolder;
        private readonly CreateTaskDtoMapper DtoMapperCreateTask;
        private readonly CreateFolderDtoMapper DtoMapperCreateFolder;
        private readonly ForgivingFormatWithRegexProcessor TaskNLP;

        /// <summary>
        /// Initializes a new instance of the TaskController class.
        /// </summary>
        /// <param name="taskDataRepository">The implementation of ITaskDataRepository to use.</param>
        /// <param name="taskFolderRepository">The implementation of ITaskFolderRepository to use.</param>
        public TaskController(ITaskDataRepository taskDataRepository,
                                ITaskFolderRepository taskFolderRepository)
        {
            TaskDataRepository = taskDataRepository;
            TaskFolderRepository = taskFolderRepository;
            DtoMapperGetTask = new GetTaskDtoMapper();
            DtoMapperGetFolder = new GetFolderDtoMapper();
            DtoMapperCreateTask = new CreateTaskDtoMapper();
            DtoMapperCreateFolder = new CreateFolderDtoMapper();
        }

        /// <summary>
        /// Get all tasks managed in the system.
        /// </summary>
        /// <returns>A list of GetTaskDto objects representing tasks in the system.</returns>
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
        /// Get a task using its ID.
        /// </summary>
        /// <param name="id">The ID of the task to retrieve.</param>
        /// <returns>The GetTaskDto representing the retrieved task, or <see langword="null"/> if the task is not found.</returns>
        public GetTaskDto GetTask(string id)
        {
            var task = TaskDataRepository.FindById(id);
            if (task == null)
            {
                return null;
            }

            return DtoMapperGetTask.Map(task);
        }

        /// <summary>
        /// Retrieves a list of tasks with the specified IDs.
        /// </summary>
        /// <remarks>
        /// Use this method to retrieve task data from a list of IDs.
        /// </remarks>
        /// <param name="ids">The list of task IDs to retrieve.</param>
        /// <returns>A list of <see cref="GetTaskDto"/> objects representing the retrieved tasks.</returns>
        public List<GetTaskDto> GetTasks(List<string> ids)
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
        /// Retrieves all task folders in the system.
        /// </summary>
        /// <returns>A list of <see cref="GetFolderDto"/> objects representing the retrieved task folders.</returns>
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
        /// Retrieves a task folder with the specified ID.
        /// </summary>
        /// <param name="folderId">The ID of the task folder to retrieve.</param>
        /// <returns>The <see cref="GetFolderDto"/> representing the retrieved task folder, or null if not found.</returns>
        public GetFolderDto GetTaskFolder(string folderId)
        {
            try
            {
                var folder = GetFolderWithIdOrNameOrThrow(folderId);
                if (folder != null)
                    return DtoMapperGetFolder.Map(folder);

            }
            catch { }

            return null;
        }

        /// <summary>
        /// Counts the number of incomplete tasks in the specified folder.
        /// </summary>
        /// <remarks>
        /// This method retrieves the incomplete tasks in the folder identified by <paramref name="folderId"/> and returns the count.
        /// </remarks>
        /// <param name="folderId">The ID of the folder to count incomplete tasks in.</param>
        /// <returns>The number of incomplete tasks in the specified folder.</returns>
        /// <exception cref="Exception">Thrown if the folder with the specified ID cannot be found.</exception>
        public long CountIncomplete(string folderId)
        {
            var folder = GetFolderWithIdOrNameOrThrow(folderId);

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
        /// Sets the completion status of a task.
        /// </summary>
        /// <param name="id">The ID of the task to set completion status for.</param>
        /// <param name="completed">A boolean value indicating whether the task is completed.</param>
        /// <returns>True if the task completion status was successfully updated; otherwise, false.</returns>
        /// <exception cref="Exception">Thrown if the task with the specified ID cannot be found.</exception>
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
        /// Deletes the task with the specified ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <returns>True if the task was successfully deleted; otherwise, false.</returns>
        /// <exception cref="Exception">Thrown if the task or the folder containing the task cannot be found.</exception>
        public bool DeleteTask(string taskId)
        {
            var folders = TaskFolderRepository.FindAll();
            if (folders == null || folders.Count == 0)
                return false;

            var folder = folders.Where(f => f.TaskIds.Contains(taskId))
                                .FirstOrDefault();
            if (folder == null)
                throw new Exception($"Unable to find Folder containing Task Id: {taskId}");

            var task = TaskDataRepository.FindById(taskId);
            if (task == null)
                throw new Exception($"Unable to find Task with Id: {taskId}");

            var savedFolderId = TaskFolderRepository.Save(folder.WithoutTask(taskId));
            var didDeleteTask = TaskDataRepository.Delete(taskId);

            if (savedFolderId != null && didDeleteTask) // not great, but fits purpose
                return true;

            return false;
        }

        /// <summary>
        /// Deletes a task from the specified folder.
        /// </summary>
        /// <remarks>
        /// This method deletes the task identified by <paramref name="taskId"/> from the folder identified by <paramref name="folderId"/>.
        /// </remarks>
        /// <param name="folderId">The ID of the folder containing the task.</param>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <returns>True if the task was successfully deleted from the folder; otherwise, false.</returns>
        public bool DeleteTaskFromFolder(string folderId, string taskId)
        {
            var folder = GetFolderWithIdOrNameOrThrow(folderId);

            var task = TaskDataRepository.FindById(taskId);
            if (task == null)
                throw new Exception($"Unable to find Task with Id: {taskId}");

            var savedFolderId = TaskFolderRepository.Save(folder.WithoutTask(taskId));
            var didDeleteTask = TaskDataRepository.Delete(taskId);

            if (savedFolderId != null && didDeleteTask) // not great, but fits purpose
                return true;

            return false;
        }

        /// <summary>
        /// Creates a new task in the specified task folder.
        /// </summary>
        /// <remarks>
        /// This method creates a new task using the provided data transfer object (<paramref name="dto"/>).
        /// </remarks>
        /// <param name="dto">The data transfer object containing the task information.</param>
        /// <returns>The ID of the newly created task.</returns>
        /// <exception cref="Exception">Thrown if the task cannot be created or added to the folder.</exception>
        public string CreateTask(CreateTaskDto dto)
        {
            var folder = GetFolderWithIdOrNameOrThrow(dto.InFolderId);

            var task = DtoMapperCreateTask.Map(dto);
            var savedTask = TaskDataRepository.Save(task);
            if (savedTask == null)
                throw new Exception($"Unable to Create task in folder {dto.InFolderId}");

            var folderUpdated = folder.WithTask(savedTask.Id);
            var updatedFolderId = TaskFolderRepository.Save(folderUpdated);
            if (updatedFolderId == null)
                throw new Exception($"Unable to Create task in folder {dto.InFolderId}");

            return savedTask.Id;
        }

        /// <summary>
        /// Creates a new task folder.
        /// </summary>
        /// <remarks>
        /// This method creates a new task folder using the provided data transfer object (<paramref name="dto"/>).
        /// </remarks>
        /// <param name="dto">The data transfer object containing the folder information.</param>
        /// <returns>The ID of the newly created task folder.</returns>
        /// <exception cref="Exception">Thrown if the task folder cannot be created.</exception>
        public string CreateTaskFolder(CreateFolderDto dto)
        {
            var taskFolder = DtoMapperCreateFolder.Map(dto);
            try
            {
                var saved = TaskFolderRepository.Save(taskFolder);
                return saved.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not save Task Folder: {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes a folder, which must be empty.
        /// </summary>
        /// <remarks>
        /// This method deletes the folder identified by <paramref name="folderId"/>. The folder must not contain any tasks for deletion to succeed.
        /// </remarks>
        /// <param name="folderId">The ID of the folder to delete.</param>
        /// <returns>True if the folder was successfully deleted; otherwise, false.</returns>
        public bool DeleteTaskFolder(string folderId)
        {
            var success = TaskFolderRepository.Delete(folderId);
            if (!success)
                success = TaskFolderRepository.DeleteByName(folderId);

            return success;
        }

        /// <summary>
        /// Moves a task from one folder to another.
        /// </summary>
        /// <remarks>
        /// This method moves the task identified by <paramref name="taskId"/> from the folder identified by <paramref name="fromFolderId"/> to the folder identified by <paramref name="toFolderId"/>.
        /// </remarks>
        /// <param name="taskId">The ID of the task to move.</param>
        /// <param name="fromFolderId">The ID of the folder from which to move the task.</param>
        /// <param name="toFolderId">The ID of the folder to which the task should be moved.</param>
        /// <exception cref="Exception">Thrown if the task cannot be found or if there is an issue saving the changes to the folders.</exception>
        public bool MoveTask(string taskId, string fromFolderId, string toFolderId)
        {
            var task = TaskDataRepository.FindById(taskId);
            if (task == null)
                throw new Exception($"Could not find Task with Id: {taskId}");

            var fromFolder = GetFolderWithIdOrNameOrThrow(fromFolderId);
            var toFolder = GetFolderWithIdOrNameOrThrow(toFolderId);

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
        /// Updates modifiable properties of a task.
        /// </summary>
        /// <remarks>
        /// This method updates the specified property of the task identified by <paramref name="id"/> with the provided <paramref name="value"/>.
        /// </remarks>
        /// <param name="id">The ID of the task to update.</param>
        /// <param name="property">The name of the property to update.</param>
        /// <param name="value">The new value for the property.</param>
        /// <returns>The ID of the updated task.</returns>
        /// <exception cref="Exception">Thrown if the task cannot be found or if there is an issue saving the changes.</exception>
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
            else if (RegexUtility.Regex_DueDatePropertyNameLowerCase().IsMatch(property)) // due Date
                task = task.WithDueDate(new DateTime((long)value));
            else
                throw new Exception($"Unrecognized property: {property}");

            try
            {
                var saved = TaskDataRepository.Save(task);
                return saved.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not save Task: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates modifiable properties of a folder.
        /// </summary>
        /// <remarks>
        /// This method updates the specified property of the folder identified by <paramref name="id"/> with the provided <paramref name="value"/>.
        /// </remarks>
        /// <param name="id">The ID of the folder to update.</param>
        /// <param name="property">The name of the property to update.</param>
        /// <param name="value">The new value for the property.</param>
        /// <returns>The ID of the updated folder.</returns>
        /// <exception cref="Exception">Thrown if the folder cannot be found or if there is an issue saving the changes.</exception>
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
                var saved = TaskFolderRepository.Save(folder);
                return saved.Id;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not save Folder: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a folder with the specified identifier or name.
        /// </summary>
        /// <remarks>
        /// This method retrieves a folder from the repository based on the provided <paramref name="identifier"/>.
        /// If the folder is not found by name, it attempts to find it by ID.
        /// </remarks>
        /// <param name="identifier">The name or ID of the folder to retrieve.</param>
        /// <returns>The folder with the specified identifier.</returns>
        /// <exception cref="Exception">Thrown if the folder cannot be found.</exception>
        private TaskFolder GetFolderWithIdOrNameOrThrow(string identifier)
        {
            var folder = TaskFolderRepository.FindById(identifier);
            if (folder == null)
            {
                Debug.WriteLine($"Did not match {identifier} to a Folder id, will try lookup by Name next...");
                folder = TaskFolderRepository.FindOneByName(identifier);
                if (folder == null)
                    throw new Exception($"Unable to find Folder with Name or Id: {identifier}");
            }

            return folder;
        }
    }
}