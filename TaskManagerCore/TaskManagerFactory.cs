using BinaryFileHandler;
using TaskManagerCore.Controller;
using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TaskManagerCore.Infrastructure.BinaryFile;

namespace TaskManagerCore
{
    /// <summary>
    /// Factory class for creating instances of TaskController class.
    /// </summary>
    public class TaskManagerFactory
    {
        /// <summary>
        /// Creates a default TaskController instance with default configuration.
        /// </summary>
        /// <remarks>
        /// <para>This is suitable in most cases.</para>
        /// <para>If a more customization is required, see <see cref="TaskController"/> 
        /// for examples on how to override default behavior.</para>
        /// </remarks>
        /// <returns>A default TaskController instance.</returns>
        public static TaskController CreateDefaultTaskManager()
        {
            // Configs for file handlers for tasks and folders
            var tasksFileConf = new BinaryFileConfig("taskmanager-task-data");
            var folderFileConf = new BinaryFileConfig("taskmanager-folder-data");

            // Initialize file handlers for tasks and folders
            var taskWriter = new TaskDataFileWriter(tasksFileConf);
            var taskReader = new TaskDataFileReader(tasksFileConf);
            var folderWriter = new TaskFolderFileWriter(folderFileConf);
            var folderReader = new TaskFolderFileReader(folderFileConf);

            // Initialize DAOs and repositories for tasks and folders
            var taskDao = new TaskDataDao(taskReader, taskWriter);
            var taskRepo = new TaskDataRepository(taskDao);
            var folderDao = new TaskFolderDao(folderReader, folderWriter);
            var folderRepo = new TaskFolderRepository(folderDao);

            // Initialize TaskController with repositories
            var controller = new TaskController(taskRepo, folderRepo);

            return controller;
        }
    }
}
