using BinaryFileHandler;
using TaskManagerCore.Controller;
using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TaskManagerCore.Infrastructure.BinaryFile;
using System.IO;
using TaskManagerCore.Infrastructure;
using TaskManagerCore.Model.Repository;

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

        /// <summary>
        /// For part 6 SQL
        /// </summary>
        /// <param name="secondaryTaskRepo"></param>
        /// <param name="secondaryFolderRepo"></param>
        /// <returns></returns>
        public static TaskController CreateDualDataSourceTaskManager(ITaskDataRepository secondaryTaskRepo, ITaskFolderRepository secondaryFolderRepo)
        {
            //var dataFilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tm_data");
            var dataFilesFolderPath = Path.Combine(Path.GetTempPath(), "task-manager", "tm_data");
            Directory.CreateDirectory(dataFilesFolderPath);
            var tasksFileConf = new BinaryFileConfig("taskmanager-task-data", dataFilesFolderPath);
            var folderFileConf = new BinaryFileConfig("taskmanager-folder-data", dataFilesFolderPath);

            var taskWriter = new TaskDataFileWriter(tasksFileConf);
            var taskReader = new TaskDataFileReader(tasksFileConf);
            var folderWriter = new TaskFolderFileWriter(folderFileConf);
            var folderReader = new TaskFolderFileReader(folderFileConf);

            var taskDao = new TaskDataDao(taskReader, taskWriter);              // Use this for the BinaryFile namespace class (part2 of task)
            var taskRepo = new TaskDataRepository(taskDao);

            var folderDao = new TaskFolderDao(folderReader, folderWriter);      // Use this for the BinaryFile namespace class (part2 of task)
            var fodlerRepo = new TaskFolderRepository(folderDao);

            var dualTaskRepo = new TaskDataDualRepositoryRunner(taskRepo, secondaryTaskRepo);
            var dualFolderRepo = new TaskFolderDualRepositoryRunner(fodlerRepo, secondaryFolderRepo);

            var controller = new TaskController(dualTaskRepo, dualFolderRepo);

            return controller;
        }
    }
}
