using Tp = TaskManagerConsoleApp.ConsoleHelpers.ConsoleHelperForTaskController;
using Ts = TaskManagerConsoleApp.ConsoleHelpers.SimpleBehaviorTests;
using Conv = TaskManagerConsoleApp.ConsoleHelpers.TaskManagerDtoToGenericConverters;
using TaskManagerCore.Controller;
using TaskManagerCore.Infrastructure.BinaryFile;
using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TaskManagerCore.Model.Dto;
using BinaryFileHandler;
using TaskManagerCore.SQL.Sqlite.Dao;
using TaskManagerCore.SQL.Sqlite;
using TaskManagerCore.Infrastructure;
using System.Threading.Tasks;

namespace TaskManagerConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /**
             * Construct application graph / bootstrap
             */
            var dataFilesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tm_data");
            Directory.CreateDirectory(dataFilesFolderPath);
            var tasksFileConf = new BinaryFileConfig("taskmanager-task-data", dataFilesFolderPath);
            var folderFileConf = new BinaryFileConfig("taskmanager-folder-data", dataFilesFolderPath);

            var taskWriter = new TaskDataFileWriter(tasksFileConf);
            var taskReader = new TaskDataFileReader(tasksFileConf);
            var folderWriter = new TaskFolderFileWriter(folderFileConf);
            var folderReader = new TaskFolderFileReader(folderFileConf);

            /**
             * Infrastructure: Persistence stuff (In-memory data store)
             */
            //var taskDao = new TaskDataDao();                                  // Use this for the Memory namespace class (part1 of task)
            //var folderDao = new TaskFolderDao();                              // Use this for the Memory namespace class (part1 of task)

            /**
             * Infrastructure: Persistence stuff (Binary file data store)
             */
            var taskDao = new TaskDataDao(taskReader, taskWriter);              // Use this for the BinaryFile namespace class (part2 of task)
            var taskRepo = new TaskDataRepository(taskDao);
            
            var folderDao = new TaskFolderDao(folderReader, folderWriter);      // Use this for the BinaryFile namespace class (part2 of task)
            var fodlerRepo = new TaskFolderRepository(folderDao);

            /**
             * Infrastructure: Persistence stuff (Sql data store)
             */
            var dbContext = new SqliteContext(dataFilesFolderPath);
            //dbContext.Database.EnsureCreated(); // shifted inside SqliteContext

            var taskDataDaoSql = new TaskDataSqlDao(dbContext);
            var taskRepoSql = new TaskDataSqlRepository(taskDataDaoSql);

            var taskFolderDaoSql = new TaskFolderSqlDao(dbContext);
            var folderRepoSql = new TaskFolderSqlRepository(taskFolderDaoSql);

            /**
             * Infrastructure: Persistence stuff (Dual Repository Repository)
             */
            var dualTaskRepo = new TaskDataDualRepositoryRunner(taskRepo, taskRepoSql);
            var dualFolderRepo = new TaskFolderDualRepositoryRunner(fodlerRepo, folderRepoSql);

            /**
             * Task Controller instance
             * (Main controller class to all functionality)
             */
            var controller = new TaskController(dualTaskRepo, dualFolderRepo);

            /** 
             * Console UI
             * (Temporary UI for messing around with)
             */
            BuildAndRunConsoleUI(controller);
        }

        static void BuildAndRunConsoleUI(TaskController controller)
        {
            // async display title screen here while application loads ?
            var task = Task.Run(() => {
                Tp.PrintTitleAscii();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\nFor the best viewing experience, please widen the console until the Title displays correctly.\n");
                Console.ResetColor();

                Tp.ClearScreen();
                //Tp.PrintTitle();
            });

            // Construct Console UI
            var funcGetAllFolders = () => Conv.ConvertFolderDtosToGenericData(controller.GetTaskFolders());
            var funcCreateFolder = (string name) => controller.CreateTaskFolder(new CreateFolderDto(name));
            var funcDeleteFolder = controller.DeleteTaskFolder;
            var funcGetTasksByIds = (List<string> ids) => Conv.ConvertTaskDtosToGenericData(controller.GetTasks(ids));
            var funcGetTaskById = (string id) => Conv.ConvertTaskDtoToGenericData(controller.GetTask(id));
            var funcGetAllTasks = () => Conv.ConvertFolderDtosToGenericData(controller.GetTaskFolders());
            var funcDeleteTask = controller.DeleteTaskFromFolder;
            var funcCompleteTask = (string id, bool completed) => controller.CompleteTask(id, completed);
            var funcUpdateTaskProperty = controller.UpdateTaskProperty;
            var funcCreateTask = (Dictionary<string, object> taskData) => controller.CreateTask(Conv.ConvertGenericDataToTaskDto(taskData));
            var funcGetFolderById = (string id) => Conv.ConvertFolderDtoToGenericData(controller.GetTaskFolder(id));

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