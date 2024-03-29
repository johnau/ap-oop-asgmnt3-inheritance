using BinaryFileHandler;
using TaskManagerCore.Controller;
using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TaskManagerCore.Infrastructure.BinaryFile;

namespace TaskManagerCore
{
    public class TaskManagerFactory
    {
        public static TaskController CreateDefaultTaskManager()
        {
            var tasksFileConf = new BinaryFileConfig("taskmanager-task-data");
            var folderFileConf = new BinaryFileConfig("taskmanager-folder-data");

            var taskWriter = new TaskDataFileWriter(tasksFileConf);
            var taskReader = new TaskDataFileReader(tasksFileConf);
            var folderWriter = new TaskFolderFileWriter(folderFileConf);
            var folderReader = new TaskFolderFileReader(folderFileConf);

            //var taskDao = new TaskDataDao();                                  // Use this for the Memory namespace class (part1 of task)
            var taskDao = new TaskDataDao(taskReader, taskWriter);              // Use this for the BinaryFile namespace class (part2 of task)
            var taskRepo = new TaskDataRepository(taskDao);
            //var folderDao = new TaskFolderDao();                              // Use this for the Memory namespace class (part1 of task)
            var folderDao = new TaskFolderDao(folderReader, folderWriter);      // Use this for the BinaryFile namespace class (part2 of task)
            var fodlerRepo = new TaskFolderRepository(folderDao);
            var controller = new TaskController(taskRepo, fodlerRepo);


            return controller;

        }

    }
}
