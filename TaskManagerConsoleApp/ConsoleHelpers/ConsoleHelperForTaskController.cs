using TaskManagerCore.Controller;

namespace TaskManagerConsoleApp.ConsoleHelpers
{
    internal class ConsoleHelperForTaskController
    {
        internal static void PrintTasksAndFolders(TaskController controller, string message)
        {
            Console.WriteLine($"\n{message}\n{new string('=', 62)}");
            var folders = controller.GetTaskFolders();
            var tasks = controller.GetTasks();
            foreach (var item in folders)
            {
                var incompleteTaskCount = controller.CountIncomplete(item.Id);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Folder: {item.Name,-20} contains {item.TaskIds.Count} Tasks ({incompleteTaskCount} incomplete)");
                Console.ResetColor();
            }
            Console.WriteLine(new string('-', 62));
            foreach (var task in tasks)
            {
                var dd = task.DueDate != null ? string.Format("{0:yyyy-MM-dd HH:mm:ss}", task.DueDate) : "None";
                Console.ForegroundColor = task.Overdue ? ConsoleColor.Red : ConsoleColor.Green;
                Console.WriteLine($"Task[{task.Type}]: {task.Description,-22} \t| Notes: {task.Notes,-50} \t| Completed: {task.Completed} \t| DueDate: {dd,-20} \t| Overdue: {task.Overdue}");
                Console.ResetColor();
            }
        }

        internal static void PrintTitle()
        {
            int width = Console.WindowWidth;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Green;
            var title = "Task Manager";
            Console.WriteLine(title + new string(' ', width - title.Length));
            Console.ResetColor();
            Console.WriteLine(' ');
        }

        internal static string Prompt_CreateActions()
        {
            Console.WriteLine("Create:");
            Console.WriteLine("T\t\tCreate a new Task");
            Console.WriteLine("F\t\tCreate a new Folder");
            Console.WriteLine("C\t\tCancel");
            var input = Console.ReadLine();
            if (input == null) return "cancel";

            switch (input.ToUpper())
            {
                case "T":
                    Console.WriteLine($"Creating new Task...");
                    throw new NotImplementedException();
                case "F":
                    Console.WriteLine($"Creating new Folder...");
                    throw new NotImplementedException();
                case "C":
                    Console.WriteLine($"Cancelling");
                    throw new NotImplementedException();
                default:
                    Console.WriteLine("Unrecognized command, try again.");
                    return Prompt_CreateActions();
            }
        }

        internal static void PrintTitleAscii()
        {
            string[] asciiArt = new string[]
            {
                "░▒▓████████▓▒░▒▓██████▓▒░ ░▒▓███████▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓██████████████▓▒░ ░▒▓██████▓▒░░▒▓███████▓▒░ ░▒▓██████▓▒░ ░▒▓██████▓▒░░▒▓████████▓▒░▒▓███████▓▒░  ",
                "   ░▒▓█▓▒░  ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ ",
                "   ░▒▓█▓▒░  ░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ ",
                "   ░▒▓█▓▒░  ░▒▓████████▓▒░░▒▓██████▓▒░░▒▓███████▓▒░       ░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓████████▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓████████▓▒░▒▓█▓▒▒▓███▓▒░▒▓██████▓▒░ ░▒▓███████▓▒░  ",
                "   ░▒▓█▓▒░  ░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ ",
                "   ░▒▓█▓▒░  ░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░ ",
                "   ░▒▓█▓▒░  ░▒▓█▓▒░░▒▓█▓▒░▒▓███████▓▒░░▒▓█▓▒░░▒▓█▓▒░      ░▒▓█▓▒░░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░▒▓█▓▒░░▒▓█▓▒░░▒▓██████▓▒░░▒▓████████▓▒░▒▓█▓▒░░▒▓█▓▒░ ",
            };

            foreach (var line in asciiArt)
            {
                foreach (char c in line)
                {
                    Console.Write(c);
                }
                Console.WriteLine();
                Thread.Sleep(50);
            }
            Thread.Sleep(500);
        }

        internal static void ClearScreen()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
