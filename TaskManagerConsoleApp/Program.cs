using Tp = TaskManagerConsoleApp.ConsoleHelpers.ConsoleHelperForTaskController;
using Ts = TaskManagerConsoleApp.ConsoleHelpers.SimpleBehaviorTests;
using Conv = TaskManagerConsoleApp.ConsoleHelpers.TaskManagerDtoToGenericConverters;
using TaskManagerCore.Controller;
//using TaskManagerCore.Infrastructure.Memory;
//using TaskManagerCore.Infrastructure.Memory.Dao;
using TaskManagerCore.Infrastructure.BinaryFile;
using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Dto;

namespace TaskManagerConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // async display title screen here while application loads ?
            var task = Task.Run(() => {
                Tp.PrintTitleAscii();
                Tp.ClearScreen();
                //Tp.PrintTitle();
            });

            // construct application
            var taskDataFileName = "taskmanager-task-data";
            var folderDataFileName = "taskmanager-folder-data";
            var taskWriter = new TaskDataFileWriter(taskDataFileName);
            var taskReader = new TaskDataFileReader(taskDataFileName);
            var folderWriter = new TaskFolderFileWriter(folderDataFileName);
            var folderReader = new TaskFolderFileReader(folderDataFileName);

            var entityFactory = new EntityFactory(); // exposed internals to TaskManagerConsoleApp namespace
            //var taskDao = new TaskDataDao();                                  // Use this for the Memory namespace class (part1 of task)
            var taskDao = new TaskDataDao(taskReader, taskWriter);              // Use this for the BinaryFile namespace class (part2 of task)
            var taskRepo = new TaskDataRepository(taskDao, entityFactory);
            //var folderDao = new TaskFolderDao();                              // Use this for the Memory namespace class (part1 of task)
            var folderDao = new TaskFolderDao(folderReader, folderWriter);      // Use this for the BinaryFile namespace class (part2 of task)
            var fodlerRepo = new TaskFolderRepository(folderDao, entityFactory);
            var controller = new TaskController(taskRepo, fodlerRepo);

            //RunTests(controller);

            var funcGetAllFolders = () => Conv.ConvertFolderDtosToGenericData(controller.GetTaskFolders());
            var funcCreateFolder = (string name) => controller.CreateTaskFolder(new CreateFolderDto(name));
            var funcDeleteFolder = controller.DeleteTaskFolder;
            var funcGetTasksByIds = (List<string> ids) => Conv.ConvertTaskDtosToGenericData(controller.GetTasksByIds(ids));
            var funcGetTaskById = (string id) => Conv.ConvertTaskDtoToGenericData(controller.GetTaskById(id));
            var funcGetAllTasks = () => Conv.ConvertFolderDtosToGenericData(controller.GetTaskFolders());
            var funcDeleteTask = controller.DeleteTaskFromFolder;
            var funcCompleteTask = (string id, bool completed) => controller.CompleteTask(id, completed);
            var funcUpdateTaskProperty = controller.UpdateTaskProperty;
            var funcCreateTask = (Dictionary<string, object> taskData) => controller.CreateTask(Conv.ConvertGenericDataToTaskDto(taskData));
            var funcGetFolderById = (string id) => Conv.ConvertFolderDtoToGenericData(controller.GetTaskFolderById(id));

            var console = new TaskManagerConsoleHandler(
                func_GetAllFoldersAsList: funcGetAllFolders, 
                func_CreateFolderById: funcCreateFolder,
                func_DeleteFolderById: funcDeleteFolder,
                func_GetTaskById: funcGetTaskById,
                func_GetTasksByIds: funcGetTasksByIds,
                func_GetAllTasksAsList: funcGetAllTasks,
                func_DeleteTask: funcDeleteTask,
                func_CompleteTask: funcCompleteTask,
                func_UpdateTaskProperty: funcUpdateTaskProperty,
                func_CreateTask: funcCreateTask,
                func_GetFolderById: funcGetFolderById
            );

            task.Wait();
            console.Display_MainMenu();
        }

        static void RunTests(TaskController controller)
        {            
            // Simple run through of behvior for testing
            // Replace all this with actual Console UI

            try
            {
                Ts.PopulateDummyData(controller); // populate some dummy tasks and folders to the "database"
            }
            catch (Exception)
            {
                Console.WriteLine("Database is already populated - not running PopulateDummyData() method");
            }

            Tp.PrintTasksAndFolders(controller, "Loaded Application");

            Ts.ChangeData(controller);

            Tp.PrintTasksAndFolders(controller, "Updated Data");

            // ------------

        }

    }
}