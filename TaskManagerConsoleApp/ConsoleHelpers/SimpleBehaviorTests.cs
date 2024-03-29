using TaskManagerCore.Controller;
using TaskManagerCore.Model.Dto;
using TaskManagerCore.Model;
using Tp = TaskManagerConsoleApp.ConsoleHelpers.ConsoleHelperForTaskController;

namespace TaskManagerConsoleApp.ConsoleHelpers
{
    internal class SimpleBehaviorTests
    {

        internal static void ChangeData(TaskController controller)
        {
            var allTasks = controller.GetTasks();
            foreach (var item in allTasks)
            {
                controller.CompleteTask(item.Id);
            }
        }

        internal static void PopulateDummyData(TaskController controller)
        {
            // tests

            // Work folder
            var workFolder = new CreateFolderDto("Work");
            var workFolder_Id = controller.CreateTaskFolder(workFolder); // may throw Exception
            Console.WriteLine($"Work Folder Id=\t\t{workFolder_Id}");

            // Work folder Tasks
            var workTask1 = new CreateTaskDto(TaskType.SINGLE, workFolder_Id, "Important meeting!", "Don't forget to bring a towel.", DateTime.Parse("2023-04-01 10:15:00"));
            var workTask1_Id = controller.CreateTask(workTask1);
            Console.WriteLine($"Work Task #1 Id=\t{workTask1_Id}");
            var workTask2 = new CreateTaskDto(TaskType.SINGLE, workFolder_Id, "Unimportant meeting", "Maybe I'll go, maybe I won't", DateTime.Parse("2023-04-02 11:30:00"));
            var workTask2_Id = controller.CreateTask(workTask2);
            Console.WriteLine($"Work Task #2 Id=\t{workTask2_Id}");

            // Personal folder
            var personalFolder = new CreateFolderDto("Personal");
            var personalFolder_Id = controller.CreateTaskFolder(personalFolder); // may throw Exception
            Console.WriteLine($"Personal Folder Id=\t{personalFolder_Id}");

            // Personal folder Tasks
            var personalTask1 = new CreateTaskDto(TaskType.SINGLE, personalFolder_Id, "Doctor appointment", "Don't forget to tell her about that weird mole", DateTime.Parse("2024-03-20 14:30:00"));
            var personalTask1_Id = controller.CreateTask(personalTask1);
            Console.WriteLine($"Personal Task #1 Id=\t{personalTask1_Id}");
            var personalTask2 = new CreateTaskDto(TaskType.SINGLE, personalFolder_Id, "Watch latest blockbuster", "Think it's another one of those superhero movies", DateTime.Parse("2024-03-03 17:30:00"));
            var personalTask2_Id = controller.CreateTask(personalTask2);
            Console.WriteLine($"Personal Task #2 Id=\t{personalTask2_Id}");
            var personalTask3 = new CreateTaskDto(TaskType.SINGLE, personalFolder_Id, "New Task", "");
            var personalTask3_Id = controller.CreateTask(personalTask3);
            Console.WriteLine($"Personal Task #3 Id=\t{personalTask3_Id}");

            // Miscellaneous folder
            var miscFolder = new CreateFolderDto("Miscellaneous");
            var miscFolder_Id = controller.CreateTaskFolder(miscFolder); // may throw Exception
            Console.WriteLine($"Misc Folder Id=\t\t{miscFolder_Id}");

            // Miscellenous folder Tasks
            var miscTask1 = new CreateTaskDto(TaskType.SINGLE, miscFolder_Id, "Check the thing", "Don't forget to check the thing...", DateTime.Parse("2023-06-30 03:00:00"));
            var miscTask1_Id = controller.CreateTask(miscTask1);
            Console.WriteLine($"Misc Task #1 Id=\t{miscTask1_Id}");

            // Check
            Tp.PrintTasksAndFolders(controller, "Created folders and tasks; Work -> 2, Personal -> 3, Misc -> 1");

            // Complete Tasks
            var workTask1_Completed = controller.CompleteTask(workTask1_Id);
            var miscTask1_Completed = controller.CompleteTask(miscTask1_Id);

            // Delete Tasks
            var personalTask3_deleted = controller.DeleteTaskFromFolder(personalFolder_Id, personalTask3_Id);

            // Check
            Tp.PrintTasksAndFolders(controller, "Completed Work Task 1 and Misc Task 1, Deleted Personal Task 3");

            //Prompt_CreateActions();

            // Repeating Tasks
            var personalTask4 = new CreateTaskDto(TaskType.REPEATING, personalFolder_Id, "Go for a run", "Daily fitness exercise", DateTime.Parse("2024-03-20 14:30:00"), TimeInterval.Daily);
            var personalTask4_Id = controller.CreateTask(personalTask4);

            Tp.PrintTasksAndFolders(controller, "Repeating tasks");
        }
    }
}
