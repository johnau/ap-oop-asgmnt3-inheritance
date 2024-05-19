using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using TaskManagerConsoleApp.ConsoleHelpers;
using TaskManagerCore.Model;

namespace TaskManagerConsoleApp
{
    /// <summary>
    /// Simple Console UI that seemed like it could be kept simple without getting into a full templating system
    /// Ends up more messing around than doing it properly
    /// Methods prefixed with "Display_" are views
    /// Methods prefixed with "Render" are components
    /// 
    /// TODO: 
    /// - sanitize user inputs (", \, etc)
    /// - break all of this apart into components (page, table, menu, menuitem, action, etc) (or just an existing console framework like a sane person)
    /// - implement cell coloring to the tables
    /// </summary>
    internal class TaskManagerConsoleHandler
    {
        int _maxRows; // to be used for pagination

        Func<List<Dictionary<string, object>>> GetAllFoldersList;
        Func<string, bool> DeleteFolder;
        Func<string, string> CreateFolder;
        Func<List<Dictionary<string, object>>> GetAllTasksList;
        Func<List<string>, List<Dictionary<string, object>>> GetTasksListById;
        Func<string, Dictionary<string, object>> GetTaskById;
        Func<string, string, bool> DeleteTask;
        Func<string, bool, bool> CompleteTask;
        Func<string, string, object, string> UpdateTaskProperty;
        Func<Dictionary<string, object>, string> CreateTask;
        Func<string, Dictionary<string, object>> GetFolderById;
        public TaskManagerConsoleHandler(
            Func<List<Dictionary<string, object>>> func_GetAllFoldersAsList,
            Func<string, string> func_CreateFolderById,
            Func<string, bool> func_DeleteFolderById,
            Func<List<string>, List<Dictionary<string, object>>> func_GetTasksByIds,
            Func<string, Dictionary<string, object>> func_GetTaskById,
            Func<List<Dictionary<string, object>>> func_GetAllTasksAsList,
            Func<string, string, bool> func_DeleteTask,
            Func<string, bool, bool> func_CompleteTask,
            Func<string, string, object, string> func_UpdateTaskProperty,
            Func<Dictionary<string, object>, string> func_CreateTask,
            Func<string, Dictionary<string, object>> func_GetFolderById,
            int maxRows = 20)
        {
            _maxRows = maxRows;

            GetAllFoldersList = func_GetAllFoldersAsList;
            DeleteFolder = func_DeleteFolderById;
            CreateFolder = func_CreateFolderById;
            GetTaskById = func_GetTaskById;
            GetTasksListById = func_GetTasksByIds;
            GetAllTasksList = func_GetAllTasksAsList;
            DeleteTask = func_DeleteTask;
            CompleteTask = func_CompleteTask;
            UpdateTaskProperty = func_UpdateTaskProperty;
            CreateTask = func_CreateTask;
            GetFolderById = func_GetFolderById;
        }

        string CellStr(string value, int p1, int p2) => $"{new string(' ', p1)}{value}{new string(' ', p2)}|";
        string CellStr(string value, int padding) => $"{new string(' ', padding / 2)}{value}{new string(' ', padding / 2 + padding % 2)}|";
        string TruncateStr(string s, int maxLen) => s.Length <= maxLen ? s : string.Concat(s.AsSpan(0, maxLen - 3), "...");
        string BooleanShort(bool b) => b ? "Y" : "N";
        string BooleanBad(bool b) => b ? "X" : "";
        string BooleanLong(bool b) => b ? "Yes" : "No";
        public void Display_MainMenu()
        {
            RenderTitleBar();

            RenderTitle("Main Menu", ConsoleColor.Green);

            var commands = RenderChoices(UIData.MainMenu);

            string response = ForceUserInput(commands);
            switch (response)
            {
                case UIData.Command_AllFoldersView:
                    RenderLine("Viewing folders command triggered");
                    RenderBlank();
                    Display_FoldersView();
                    return;
                case UIData.Command_Exit:
                    if (UserConfirmChoice("Are you sure you want to exit?"))
                    {
                        Environment.Exit(0);
                        return;
                    }
                    RenderBlank();
                    Display_MainMenu();
                    return;
                default:
                    throw new Exception("There was no response...");
            }
        }

        #region Folders

        public void Display_FoldersView()
        {
            RenderTitleBar();
            var title = "Task Folders Listing";
            var folders = GetAllFoldersList.Invoke();
            Debug.WriteLine($"Got all {folders.Count} folders");
            RenderFoldersTable(folders, title);

            var commands = RenderChoices(UIData.FoldersViewMenu);

            // prompt user for response
            string response = ForceUserInput(commands);
            switch (response)
            {
                case UIData.Command_Back:
                    BackToMainMenu();
                    return;
                case UIData.Command_DeleteFolder:
                    Display_Folders_DeleteFolder(folders);
                    break;
                case UIData.Command_CreateFolder:
                    Display_Folders_CreateFolder(folders);
                    break;
                case UIData.Command_ViewFolder:
                    Display_Folders_ViewFolder(folders);
                    break;
                default:
                    throw new Exception("There was no response...");
            }

            RenderBlank();
            Display_FoldersView();
        }

        void Display_Folders_ViewFolder(List<Dictionary<string, object>> folders)
        {
            RenderBlank();
            RenderTitleBar();
            RenderFoldersTable(folders, "Select Folder to View...", ConsoleColor.Blue, ConsoleColor.Blue);
            var folderIndex = -1;
            while (folderIndex < 0 || folderIndex >= folders.Count)
            {
                RenderLine("Folder Index to View: (or `c` to cancel)");
                var viewIndex = GetUserInputLine();
                if (viewIndex == null || viewIndex == string.Empty) viewIndex = "0"; // must set to 0 here
                if (viewIndex.ToLower()[0] == 'c') return;

                folderIndex = GeneralHelpers.ConvertToIntOrZero(viewIndex) - 1;
                if (folderIndex < 0 || folderIndex >= folders.Count)
                {
                    RenderBlank();
                    RenderTitleBar(ConsoleColor.Red);
                    RenderFoldersTable(folders, "Select Folder to View...", ConsoleColor.Blue, ConsoleColor.Blue);
                    RenderLine("Invalid Folder Index to view, please try again...", ConsoleColor.Red);
                }
            }
            var folderId = (string)folders[folderIndex]["id"];
            var folderName = (string)folders[folderIndex][UIData.PropertyName_Name];
            var taskIds = (string[])folders[folderIndex][UIData.PropertyName_TaskIds];
            //var taskCount = (int)folders[folderIndex][UIData.PropertyName_TaskCount];
            //var tasksInFolder = GetTasksListById(taskIds);

            Display_Folders_ViewFolder_ViewTasksInFolder(folderId, folderName, taskIds.ToList()); // these levels are getting a bit deep... and this is setup a bit strangely - cant exit out of it and land back in the table
        }

        //void Display_Folders_ViewFolder_ViewTasksInFolder(string folderId, string folderName, List<Dictionary<string, object>> tasks)
        void Display_Folders_ViewFolder_ViewTasksInFolder(string folderId, string folderName, List<string> taskIds)
        {
            var tasks = GetTasksListById(taskIds);
            RenderBlank();
            RenderTitleBar();
            RenderTasksTable(tasks, $"Tasks in folder '{folderName}' ({tasks.Count} total)");

            // display choices View Task, Create Task, Delete Task, Go Back (to folders list)
            var commands = RenderChoices(UIData.TasksViewMenu);

            // prompt user for response
            string response = ForceUserInput(commands);
            switch (response)
            {
                case UIData.Command_Back:
                    RenderBlank();
                    //Display_FoldersView(); // ugly... need to build a hierarchy for these methods
                    return;
                case UIData.Command_DeleteTask:
                    Display_Folders_ViewFolder_DeleteTask(folderId, tasks);
                    taskIds = ((string[])GetFolderById(folderId)[UIData.PropertyName_TaskIds]).ToList();
                    break;
                case UIData.Command_CreateTask:
                    Display_Folders_ViewFolder_CreateTask(folderId, folderName, tasks);
                    taskIds = ((string[])GetFolderById(folderId)[UIData.PropertyName_TaskIds]).ToList();
                    break;
                case UIData.Command_ViewTask:
                    Display_Folders_ViewFolder_ViewTask(folderId, folderName, tasks);
                    taskIds = ((string[])GetFolderById(folderId)[UIData.PropertyName_TaskIds]).ToList();
                    break;
                default:
                    throw new Exception("There was no response...");
            }

            RenderBlank();
            Display_Folders_ViewFolder_ViewTasksInFolder(folderId, folderName, taskIds);
        }

        void Display_Folders_ViewFolder_CreateTask(string folderId, string folderName, List<Dictionary<string, object>> tasks)
        {
            var display = () =>
            {
                RenderBlank();
                RenderTitleBar(ConsoleColor.Green);
                RenderTitle($"'{folderName}' > Create New Task...", ConsoleColor.Green);
            }; display();

            var existingTaskDescriptions = tasks
                    .Where(task => task.ContainsKey(UIData.PropertyName_Description) && task[UIData.PropertyName_Description] is string)
                    .Select(task => (string)task[UIData.PropertyName_Description])
                    .ToHashSet();

            Dictionary<string, string> choices = new Dictionary<string, string>()
            {
                { UIData.Label_SingleTask, UIData.Label_SingleTask},
                { UIData.Label_RepeatingTask, UIData.Label_RepeatingTask},
                { UIData.Label_HabitualTask, UIData.Label_HabitualTask},
            };
            var commands = RenderChoices(choices);
            string response = ForceUserInput(commands);
            var taskType = response;
            var taskTypeInt = 0;
            switch (response)
            {
                case UIData.Label_SingleTask:
                    taskTypeInt = 1;
                    break;
                case UIData.Label_RepeatingTask:
                    taskTypeInt = 2;
                    break;
                case UIData.Label_HabitualTask:
                    taskTypeInt = 3;
                    break;
                default:
                    throw new Exception("Unsupported operation");
            }
            Debug.WriteLine($"Task type: {taskType} ({taskTypeInt})");

            // description input
            var taskDescription = UserEnterText("a Description", existingTaskDescriptions, display);
            if (taskDescription == null) return; // cancelled
            Debug.WriteLine($"Description: {taskDescription}");

            // notes input
            var taskNotes = UserEnterText("notes about the Task", null, display);
            if (taskNotes == null) return; // cancelled
            Debug.WriteLine($"Notes: {taskNotes}");

            // due date input
            DateTime? taskDueDate = null;
            while (true) // infinite loop until valid date response
            {
                var taskDueDateStr = UserEnterText("a Due Date (dd-MM-yyyy hh:mm AM) or leave blank for none", null, display, true);
                if (taskDueDateStr == null) return; // cancelled
                if (taskDueDateStr == string.Empty)
                {
                    break; // leave taskDueDate as null
                }
                else
                {
                    if (DateTime.TryParse(taskDueDateStr, out var _taskDueDate))
                    {
                        taskDueDate = _taskDueDate;
                        break;
                    }
                }
            }
            var taskDueDateTicks = taskDueDate.HasValue ? taskDueDate.Value.Ticks : 0L;
            Debug.WriteLine($"Due Date: {taskDueDate}");

            var taskIntervalInt = 0;
            if (taskTypeInt > 1) // prompt for interval
            {
                Dictionary<string, string> intervals = new Dictionary<string, string>()
                {
                    { "1", "Hourly" },
                    { "24", "Daily" },
                    { 24*7+"", "Weekly" },
                    { 24*7*2+"", "Fortnightly" },
                    { 24*30+"", "Monthly" },
                    { 24*365+"", "Yearly" },
                };
                var values = RenderChoices(intervals);
                string intervalStr = ForceUserInput(values);
                if (int.TryParse(intervalStr, out int _intervalInt)) taskIntervalInt = _intervalInt;
            }

            var newTaskData = new Dictionary<string, object>()
            {
                { UIData.PropertyName_InFolderId, folderId},
                { UIData.PropertyName_TaskType, taskTypeInt },
                { UIData.PropertyName_Description, taskDescription },
                { UIData.PropertyName_Notes, taskNotes },
                { UIData.PropertyName_DueDate, taskDueDateTicks },
                { UIData.PropertyName_Interval, taskIntervalInt},
            };

            //var newId = CreateTask.Invoke(newTaskData);
            string newId;
            try
            {
                newId = CreateTask.Invoke(newTaskData);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is ArgumentException)
            {
                newTaskData[UIData.PropertyName_DueDate] = DateTime.Now.AddDays(1).Ticks;
                newId = CreateTask.Invoke(newTaskData);
            }

            RenderContinuePrompt($"\nCreated New Task...\n{newId}\nType={taskTypeInt}, \nDesc='{taskDescription}', \nNote='{taskNotes}', \nDueDate='{taskDueDateTicks}', \nInterval='{taskIntervalInt}'\n");
        }

        void Display_Folders_ViewFolder_ViewTask(string folderId, string folderName, List<Dictionary<string, object>> tasks)
        {
            var display = () =>
            {
                RenderBlank();
                RenderTitleBar(ConsoleColor.Blue);
                RenderTasksTable(tasks, "Select Task to View...", ConsoleColor.Blue, ConsoleColor.Blue);
            };
            display();

            var taskIndex = UserChooseNumericOptionWithCancel("Task", "View", tasks.Count, display);
            if (taskIndex == -1) { return; } // User Cancelled

            var task = tasks[taskIndex];
            if (!task.TryGetValue("id", out var taskId)) throw new Exception("Id should not be null");

            task.TryGetValue(UIData.PropertyName_Description, out var description);

            // display task view
            Display_Task(folderId, folderName, task);
        }

        void Display_Folders_ViewFolder_DeleteTask(string folderId, List<Dictionary<string, object>> tasks)
        {
            var display = () =>
            {
                RenderBlank();
                RenderTitleBar(ConsoleColor.Red);
                RenderTasksTable(tasks, "Select Task to Delete...", ConsoleColor.Red, ConsoleColor.Red);
            };
            display();

            var taskIndex = UserChooseNumericOptionWithCancel("Task", "Delete", tasks.Count, display);
            if (taskIndex == -1) { return; } // User Cancelled

            var task = tasks[taskIndex];
            if (!task.TryGetValue("id", out var taskId)) throw new Exception("Id should not be null");

            task.TryGetValue(UIData.PropertyName_Description, out var description);

            if (UserConfirmChoice($"Are you sure you want to delete task: '{description}'"))
            {
                DeleteTask.Invoke(folderId, (string)taskId);
                RenderLine($"\nDeleted task: {description}");
            }
            else
            {
                RenderLine($"\nCanceled delete task: {description}");
            }
            RenderContinuePrompt();
        }

        void Display_Folders_CreateFolder(List<Dictionary<string, object>> folders)
        {
            HashSet<string> existingFolderNames = new HashSet<string>();
            foreach (var folder in folders)
            {
                if (!folder.ContainsKey(UIData.PropertyName_Name))
                    break;
                existingFolderNames.Add((string)folder[UIData.PropertyName_Name]);
            }

            var displayFunc = () => RenderFoldersTable(folders, "Create a new folder...", ConsoleColor.Cyan, ConsoleColor.Green);
            var folderName = UserEnterText("a name for the new folder", existingFolderNames, displayFunc);
            if (folderName == null)
            {
                return; // user cancelled
            }
            //var folderName = string.Empty;
            //var error = string.Empty;
            //while (folderName == string.Empty)
            //{
            //    RenderBlank();
            //    RenderTitleBar();
            //    RenderFoldersTable(folders, "Create a new folder...", ConsoleColor.Cyan, ConsoleColor.Green);

            //    if (error != string.Empty)
            //    {
            //        RenderLine(error, ConsoleColor.Red);
            //        error = string.Empty;
            //    }

            //    RenderLine("Enter a name for the new folder: (or `c` to cancel)");
            //    var userInput = GetUserInputLine();
            //    if (userInput == null || userInput == string.Empty)
            //    {
            //        error = "Did not provide a name, try again....";
            //        continue;
            //    }
            //    if (userInput.ToLower()[0] == 'c')
            //    {
            //        return;
            //    }
            //    if (existingFolderNames.Contains(userInput, StringComparer.OrdinalIgnoreCase))
            //    {
            //        error = "Name is in use, try again...";
            //        continue; // check if name is used
            //    }
            //    folderName = userInput;
            //}
            CreateFolder(folderName);
            RenderBlank();
            RenderTitleBar();
            RenderLine($"Created a new folder: '{folderName}'...\n", ConsoleColor.Green);
            RenderContinuePrompt();
        }

        void Display_Folders_DeleteFolder(List<Dictionary<string, object>> folders)
        {
            var display = () =>
            { // these page display components can be extracted into "view" render functions
                RenderBlank();
                RenderTitleBar(ConsoleColor.Red);
                RenderFoldersTable(folders, "Select Folder to Delete...", ConsoleColor.Red, ConsoleColor.Red);
            }; display();

            var folderIndex = UserChooseNumericOptionWithCancel("Folder", "Delete", folders.Count, display);
            if (folderIndex == -1) { return; } // User Cancelled

            var folderName = folders[folderIndex][UIData.PropertyName_Name];
            var taskCount = (int)folders[folderIndex][UIData.PropertyName_TaskCount];
            if (taskCount > 0)
            {
                RenderLine($"\nCan not delete folder containing tasks. The '{folderName}' folder contains {taskCount} tasks!\nDelete or Move these tasks and try again...\n", ConsoleColor.Yellow);
                RenderContinuePrompt("Return to folders...");
                return;
            }

            DeleteFolder.Invoke((string)folders[folderIndex]["id"]);
            RenderLine($"Deleted folder: {folderName}");
        }

        void Display_Task(string folderId, string folderName, Dictionary<string, object> taskProperties)
        {
            if (!taskProperties.TryGetValue("id", out var _taskId) || !(_taskId is string taskId)) throw new Exception("No Id for task");
            if (!taskProperties.TryGetValue(UIData.PropertyName_Description, out var description)) throw new Exception("No Description for task");
            if (!taskProperties.TryGetValue(UIData.PropertyName_TaskType, out var taskType)) taskType = "None";
            if (!taskProperties.TryGetValue(UIData.PropertyName_Notes, out var notes)) notes = string.Empty;
            if (!taskProperties.TryGetValue(UIData.PropertyName_DueDate, out var _dueDate) || !(_dueDate is long dueDate)) dueDate = -1L;

            var display = () =>
            {
                RenderBlank();
                RenderTitleBar(ConsoleColor.Green);
                RenderTaskCard(taskProperties, $"{folderName} > Task: {description} ({taskType})"); // use defaults
            }; display();

            var menu = UIData.TasksMenu;
            if (taskProperties.TryGetValue(UIData.PropertyName_Completed, out var isCompleted))
            {
                if ((bool)isCompleted)
                    menu = menu.Where((e) => e.Key != UIData.Command_Complete_Task)
                                        .ToDictionary(e => e.Key, e => e.Value);
                else
                    menu = menu.Where((e) => e.Key != UIData.Command_Uncomplete_Task)
                                        .ToDictionary(e => e.Key, e => e.Value);
            }

            var commands = RenderChoices(menu);
            string response = ForceUserInput(commands);
            switch (response)
            {
                case UIData.Command_Back:
                    RenderBlank();
                    return;
                case UIData.Command_Reload_Task:
                    taskProperties = GetTaskById(taskId); // get fresh copy of task data and refresh view (break)
                    break;
                case UIData.Command_Complete_Task:
                    if (CompleteTask.Invoke(taskId, true)) taskProperties = GetTaskById(taskId); // get fresh copy of task data and refresh view (break)
                    //taskProperties[UIData.PropertyName_Completed] = true;
                    break;
                case UIData.Command_Uncomplete_Task:
                    if (CompleteTask.Invoke(taskId, false)) taskProperties = GetTaskById(taskId); // get fresh copy of task data and refresh view (break)
                    //taskProperties[UIData.PropertyName_Completed] = false;
                    break;
                case UIData.Command_EditTaskDescription:
                    Display_Task_EditProperty(taskId, UIData.PropertyName_Description, (string)description);
                    taskProperties = GetTaskById(taskId); // get fresh copy of task data and refresh view (break)
                    break;
                case UIData.Command_EditTaskNotes:
                    Display_Task_EditProperty(taskId, UIData.PropertyName_Notes, (string)notes);
                    taskProperties = GetTaskById(taskId); // get fresh copy of task data and refresh view (break)
                    break;
                case UIData.Command_EditTaskDueDate:
                    Display_Task_EditProperty(taskId, UIData.PropertyName_DueDate, dueDate > 0L ? new DateTime(dueDate) : null);
                    taskProperties = GetTaskById(taskId); // get fresh copy of task data and refresh view (break)
                    break;
                case UIData.Command_DeleteTask:
                    if (UserConfirmChoice($"Are you sure you want to delete task: '{description}'"))
                    {
                        DeleteTask.Invoke(folderId, taskId);
                        RenderLine($"\nDeleted task: {description}");
                    }
                    else
                    {
                        RenderLine($"\nCanceled delete task: {description}");
                    }
                    RenderContinuePrompt();
                    return;
                default:
                    throw new Exception("There was no response...");
            }

            Display_Task(folderId, folderName, taskProperties); // stay in this view until users exits
        }

        void Display_Task_EditProperty(string taskId, string propertyName, string currentValue)
        {
            var newValue = UserEditProperty(propertyName, currentValue);
            if (newValue == null || newValue == currentValue) return;

            // call edit property method with propety name and new value
            var _tId = UpdateTaskProperty.Invoke(taskId, propertyName, newValue);
        }

        void Display_Task_EditProperty(string taskId, string propertyName, DateTime? currentValue)
        {
            var newValue = UserEditProperty(
                propertyName,
                currentValue.HasValue ? currentValue.Value.ToString(UIData.DateFormatString) : "None");

            if (newValue == null) return;// user cancelled

            if (newValue == string.Empty)
            {
                var noDueDate = UserConfirmChoice("Do you want to remove the Due Date from this task?");
                if (noDueDate) _ = UpdateTaskProperty.Invoke(taskId, propertyName, -1L);
                return;
            };
            if (!DateTime.TryParse(newValue, out var newDate))
            {
                RenderLine($"Invalid date format, use: {UIData.DateFormatString}");
                RenderContinuePrompt();
                return;
            }
            if (newDate == currentValue) return;

            var _tId = UpdateTaskProperty.Invoke(taskId, propertyName, newDate.Ticks);
        }
        #endregion

        #region nav
        void BackToMainMenu()
        {
            RenderBlank();
            Display_MainMenu();
        }

        #endregion

        #region User input

        /// <summary>
        /// Cancellable prompts the user to choose an index from 1 to optionsCount
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <param name="optionsCount"></param>
        /// <param name="pageDisplayFunc"></param>
        /// <returns></returns>
        int UserChooseNumericOptionWithCancel(string name, string action, int optionsCount, Action pageDisplayFunc)
        {
            var index = -1;
            while (index < 0 || index >= optionsCount)
            {
                RenderLine($"{name} Index to {action}: (or `c` to cancel)");
                var delIndex = GetUserInputLine();
                if (delIndex == null || delIndex == string.Empty) delIndex = "0"; // must set to 0 here
                if (delIndex.ToLower()[0] == 'c') return -1;

                index = GeneralHelpers.ConvertToIntOrZero(delIndex) - 1;
                if (index < 0 || index >= optionsCount)
                {
                    pageDisplayFunc.Invoke();
                    RenderLine($"Invalid {name} Index to {action}, please try again...", ConsoleColor.Red);
                }
            }
            return index;
        }

        string ForceUserInput(Dictionary<string, string> commands)
        {
            string response = string.Empty;
            while (response == string.Empty)
            {
                var _raw = GetUserInput(commands);
                if (_raw != null) response = _raw;
            }
            return response;
        }

        /// <summary>
        /// Takes a dict of choices => input, command
        /// Returns command
        /// </summary>
        /// <param name="choices"></param>
        /// <returns></returns>
        string? GetUserInput(Dictionary<string, string> choices)
        {
            Console.WriteLine("");
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            char keyPressed = keyInfo.KeyChar;

            foreach (var choice in choices)
            {
                if (keyPressed == choice.Key[0])
                {
                    Console.Write("\t");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.Write("<< User Input\n");
                    Console.ResetColor();
                    return choice.Value;
                }
            }

            RenderLine("Invalid response, try again...", ConsoleColor.Red);
            return null;
        }

        string? GetUserInputLine()
        {
            Console.WriteLine("");
            var lineInput = Console.ReadLine()?.Trim();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("^^ User Input\n");
            Console.ResetColor();
            return lineInput;
        }

        bool UserConfirmChoice(string message)
        {
            while (true)
            {
                RenderBlank();
                RenderTitleBar(ConsoleColor.Red);
                RenderTitle("Confirm choice", ConsoleColor.Red);
                RenderLine(message + " (Y/N)", ConsoleColor.Red);
                var confirm = Console.ReadKey();
                if (confirm.KeyChar == 'Y' || confirm.KeyChar == 'y') return true;
                if (confirm.KeyChar == 'N' || confirm.KeyChar == 'n') return false;
            }
        }

        /// <summary>
        /// This can be used almost like a text field
        /// </summary>
        /// <param name="description"></param>
        /// <param name="reserved"></param>
        /// <param name="displayFunc"></param>
        /// <param name="minLengthInput"></param>
        /// <returns></returns>
        string? UserEnterText(string description, HashSet<string>? reserved, Action? displayFunc = null, bool canBeEmpty = false, int minLengthInput = 2)
        {
            var folderName = string.Empty;
            var error = string.Empty;
            while (folderName == string.Empty)
            {
                RenderBlank();
                RenderTitleBar();
                displayFunc?.Invoke();

                if (error != string.Empty)
                {
                    RenderLine(error, ConsoleColor.Red);
                    error = string.Empty;
                }

                RenderLine($"Enter {description}: (or `c` to cancel)");
                var userInput = GetUserInputLine();
                Debug.WriteLine("User input: " + userInput);
                if (userInput == null || userInput == string.Empty)
                {
                    if (canBeEmpty) return string.Empty;
                    error = "Did not provide a value, try again....";
                    continue;
                }
                if (userInput.Length == 1 && userInput.ToLower()[0] == 'c')
                {
                    return null;
                }
                if (userInput.Length < minLengthInput)
                {
                    error = $"Value must be at least {minLengthInput} characters, try again....";
                    continue;
                }
                if (reserved != null && reserved.Contains(userInput, StringComparer.OrdinalIgnoreCase))
                {
                    error = $"'{userInput}' is in use, try again...";
                    continue; // check if name is used
                }
                folderName = userInput;
            }

            return folderName;
        }

        string? UserEditProperty(string propertyName, string currentValue)
        {
            var display = () =>
            { // these page display components can be extracted into "view" render functions
                RenderBlank();
                RenderTitleBar(ConsoleColor.Blue);
            }; display();

            var display2 = () =>
            {
                RenderTitle($"Edit {propertyName}:", ConsoleColor.Blue);
                RenderLine($"Current value: '{currentValue}'", ConsoleColor.Blue);
            }; display2();

            var newValue = UserEnterText($"a new {propertyName}", null, display2);
            if (newValue == null || newValue == currentValue)
            {
                return null; // user cancelled
            }
            return newValue;
        }

        #endregion

        #region Render methods - these methods only print to screen, do not mess with console state

        /// <summary>
        /// Renders the choices and returns a dict of the mappings of index to commands
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        Dictionary<string, string> RenderChoices(Dictionary<string, string> choices, int margin = 2)
        {
            var line = (string key, string label) => $"{new string(' ', margin)}{key} :\t{label}";

            var i = 0;
            var lines = new List<string>();

            var comCount = choices.Count;
            var commands = new Dictionary<string, string>();
            var hasGoBackCommand = false;
            var hasRefreshCommand = false;
            foreach (var item in choices)
            {
                if (item.Key.Equals(UIData.Command_Back))
                {
                    hasGoBackCommand = true;
                    comCount--;
                    continue; // render back command later
                }
                if (item.Key.Equals(UIData.Command_Reload_Task))
                {
                    hasRefreshCommand = true;
                    comCount--;
                    continue; // render refresh command later
                }
                if (++i > 9) break; // don't render commands past 9 (0 is reserved for back)

                commands.Add(i + "", item.Key);
                lines.Add(line(i + "", item.Value));
            }
            if (hasGoBackCommand)
            {   //render back command separately if present
                var key = "`";
                commands.Add(key, UIData.Command_Back);
                //RenderLine($"{new string(' ', margin)}{key} :\t{UIData.BackCommandLabel}");
                RenderLine(line(key, UIData.Label_BackCommand));
            }

            RenderLine("");
            lines.ForEach(line => RenderLine(line));
            RenderLine("");

            if (hasRefreshCommand)
            {   //render refresh command separately if present
                var key = "+";
                commands.Add(key, UIData.Command_Reload_Task);
                //RenderLine($"{new string(' ', margin)}{key} :\t{UIData.Label_ReloadCommand}");
                RenderLine(line(key, UIData.Label_ReloadCommand));
            }

            RenderLine($"\nChoose an option (1-{comCount}):");
            return commands;
        }

        public void RenderBlank()
        {
            Console.Clear();
        }

        public void RenderLine(string message, ConsoleColor fg = ConsoleColor.White, ConsoleColor bg = ConsoleColor.Black)
        {
            Console.ForegroundColor = fg;
            Console.BackgroundColor = bg;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        void RenderContinuePrompt(string message = "")
        {
            RenderLine(message + "[Press any key to continue...]");
            Console.ReadKey();
        }

        /// <summary>
        /// Does not control screen at all, only blits
        /// </summary>
        /// <param name="folderData"></param>
        void RenderFoldersTable(List<Dictionary<string, object>> folderData, string title = "Task Folders Listing", ConsoleColor color = ConsoleColor.White, ConsoleColor titleColor = ConsoleColor.Green)
        {
            folderData.Sort((a, b) =>
            {
                if (a.TryGetValue(UIData.PropertyName_Name, out var _nameA) && _nameA is string nameA
                && b.TryGetValue(UIData.PropertyName_Name, out var _nameB) && _nameB is string nameB)
                {
                    return nameA.CompareTo(nameB);
                }
                return 0; // ignore if nameA or nameB only is available
            });

            var margin = 2;
            var m = new string(' ', margin);
            RenderTitle(title, titleColor);

            RenderRowDivider(UIData.FoldersColumnLayout, margin);
            RenderHeader(UIData.FoldersColumnLayout, margin);
            RenderRowDivider(UIData.FoldersColumnLayout, margin);

            var index = 1;
            foreach (var folderProperties in folderData)
            {
                var row = m + "|";
                foreach (var col in UIData.FoldersColumnLayout)
                {
                    var value = folderProperties[col.Key] != null ? folderProperties[col.Key] : string.Empty;
                    value = TruncateStr(value + "", col.Value);
                    if (col.Key == UIData.PropertyName_Index) value = index;
                    var padding = Math.Max(0, col.Value - (value + "").Length);
                    row += CellStr(value + "", padding);
                    //Debug.WriteLine($"@ Column #{col.Key}");
                }
                //Debug.WriteLine($"Writing folder #{folderProperties[UIData.PropertyName_Name]} row: {row}");
                RenderLine(row, color);
                RenderRowDivider(UIData.FoldersColumnLayout, margin);
                index++;
            }
            Console.WriteLine(); // spacer after table
        }

        /// <summary>
        /// Does not control screen at all, only blits
        /// </summary>
        /// <param name="taskData"></param>
        void RenderTasksTable(List<Dictionary<string, object>> taskData, string title = "Tasks Listing", ConsoleColor color = ConsoleColor.White, ConsoleColor titleColor = ConsoleColor.Green)
        {
            taskData.Sort((a, b) =>
            { // provide other sorting through menu options (sort by completed + sort by due date) don't bother with overdue, change overdue to an asterisk on the index or something,
                if (a.TryGetValue(UIData.PropertyName_Description, out var _descA) && _descA is string descA
                && b.TryGetValue(UIData.PropertyName_Description, out var _descB) && _descB is string descB)
                {
                    return descA.CompareTo(descB);
                }
                return 0;
            });

            var margin = 2;
            var m = new string(' ', margin);
            RenderTitle(title, titleColor);

            RenderRowDivider(UIData.TasksColumnLayout, margin);
            RenderHeader(UIData.TasksColumnLayout, margin);
            RenderRowDivider(UIData.TasksColumnLayout, margin);

            var index = 1;
            foreach (var taskProperties in taskData)
            {
                var row = m + "|";
                foreach (var col in UIData.TasksColumnLayout)
                {
                    //var value = taskProperties[col.Key] != null ? taskProperties[col.Key] : string.Empty;
                    if (!taskProperties.TryGetValue(col.Key, out var value)) value = string.Empty;
                    if (col.Key.ToLower().Contains("date"))
                    {
                        if (value != null && (long)value > 0L)
                            value = new DateTime((long)value); // convert to date
                        else
                            value = "None";
                    }
                    if (col.Key.Equals(UIData.PropertyName_Overdue)) value = BooleanBad((bool)value);
                    if (col.Key == UIData.PropertyName_Index) value = index;
                    value = TruncateStr(value + "", col.Value);
                    var padding = Math.Max(0, col.Value - (value + "").Length);
                    row += CellStr(value + "", padding);
                    //Debug.WriteLine($"@ Column #{col.Key}");
                }
                //Debug.WriteLine($"Writing task #{taskProperties[UIData.PropertyName_Description]} row: {row}");
                RenderLine(row, color);
                RenderRowDivider(UIData.TasksColumnLayout, margin);
                index++;
            }
            Console.WriteLine(); // spacer after table
        }

        /// <summary>
        /// Does not control screen at all, only blits
        /// </summary>
        /// <param name="taskData"></param>
        void RenderTaskCard(Dictionary<string, object> taskProperties, string title = "Viewing Task", ConsoleColor color = ConsoleColor.White, ConsoleColor titleColor = ConsoleColor.Green)
        {
            var margin = 10;
            var m = new string(' ', margin);
            var rowStart = m + "|";
            RenderTitle($"{title}", titleColor);

            RenderRowDivider(UIData.TaskPropertiesColumnLayout, margin);
            RenderHeader(UIData.TaskPropertiesColumnLayout, margin);
            RenderRowDivider(UIData.TaskPropertiesColumnLayout, margin);

            var nameColWidth = UIData.TaskPropertiesColumnLayout[UIData.PropertyName_PropertyName];
            var valueColWidth = UIData.TaskPropertiesColumnLayout[UIData.PropertyName_PropertyValue];
            // loop through each task property
            foreach (var property in taskProperties)
            {
                var name = property.Key;
                var value = property.Value;

                if (name.Equals("id", StringComparison.OrdinalIgnoreCase)
                    || name.Equals(UIData.PropertyName_Index, StringComparison.OrdinalIgnoreCase)
                    || name.Equals(UIData.PropertyName_TaskType, StringComparison.OrdinalIgnoreCase)) continue; // dont render the id, type, index to the user

                if (name.Equals(UIData.PropertyName_Overdue, StringComparison.OrdinalIgnoreCase)) color = (bool)value ? ConsoleColor.Red : ConsoleColor.Green;
                if (name.Equals(UIData.PropertyName_Completed, StringComparison.OrdinalIgnoreCase)) color = (bool)value ? ConsoleColor.Green : ConsoleColor.DarkGray;
                if (name.Equals(UIData.PropertyName_DueDate, StringComparison.OrdinalIgnoreCase))
                {
                    color = value != null && (long)value > 0L && new DateTime((long)value) >= DateTime.Now ? ConsoleColor.White : ConsoleColor.DarkGray;
                    value = value == null || (long)value <= 0L ? "None" : new DateTime((long)value).ToString(UIData.DateFormatString);
                }

                var e = MultiLine(value + "", valueColWidth);
                var count = 1;
                while (e.MoveNext()) // iterate lines of the value
                {
                    var curr = e.Current;
                    Debug.WriteLine($"Current: {curr}");
                    var row = rowStart;

                    // render left col
                    var p1 = Math.Max(0, nameColWidth - name.Length);
                    //var p1_L = Math.Max(0, p1 / 2);
                    //var p1_R = Math.Max(0, p1 - p1_L);
                    if (count > 1) name = new string(' ', name.Length);
                    row += CellStr(name, p1 / 2, p1 / 2 + p1 % 2);

                    // render right col
                    var p2 = Math.Max(0, valueColWidth - curr.Length);
                    //var p2_L = Math.Max(0, p2 / 2);
                    //var p2_R = Math.Max(0, p2 - p2_L);
                    row += CellStr(curr + "", p2 / 2, p2 / 2 + p2 % 2);
                    RenderLine(row, color);
                    count++;
                }

                RenderRowDivider(UIData.TaskPropertiesColumnLayout, margin);
            }

            Console.WriteLine(); // spacer after table
        }

        IEnumerator<string> MultiLine(string str, int maxWidth)
        {
            var c = 0;
            if (str.Length <= maxWidth)
            {
                yield return str;
                c = str.Length;
            }
            while (c < str.Length)
            {
                var next = c + maxWidth;
                if (next > str.Length) next = str.Length;
                var line = str.Substring(c, next - c);
                c = next;
                yield return line;
            }

        }

        void RenderTitleBar(ConsoleColor bg = ConsoleColor.Green)
        {
            int width = Console.WindowWidth;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = bg;
            var title = "Task Manager";
            Console.WriteLine(title + new string(' ', width - title.Length));
            Console.ResetColor();
            Console.WriteLine(' ');
        }

        void RenderTitle(string title, ConsoleColor color)
        {
            int width = Console.WindowWidth;
            title += "\n" + new string('-', width);
            Console.ForegroundColor = color;
            Console.WriteLine(title);
            Console.ResetColor();
        }

        void RenderHeader(Dictionary<string, int> columnLayout, int margin = 0)
        {
            var m = new string(' ', margin);
            var headerRow = m + "|";
            foreach (var col in columnLayout)
            {
                var padding = Math.Max(0, col.Value - col.Key.Length);
                headerRow += CellStr(TruncateStr(col.Key, col.Value), padding);
            }
            RenderLine(headerRow, ConsoleColor.DarkGray);
        }

        void RenderRowDivider(Dictionary<string, int> columnLayout, int margin = 0)
        {
            var m = new string(' ', margin);
            var d = m + "+";
            foreach (var col in columnLayout)
            {
                d += new string('-', col.Value) + "+";
            }
            RenderLine(d, ConsoleColor.DarkGray);
        }

        #endregion
    }
}
