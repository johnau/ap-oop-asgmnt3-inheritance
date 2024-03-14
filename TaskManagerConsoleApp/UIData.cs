
namespace TaskManagerConsoleApp
{
    internal class UIData
    {
        public const string DateFormatString = "dd-MM-yyyy hh:mm tt";

        public const string Label_BackCommand = "<< Back";
        public const string Label_ReloadCommand = "Reload (*Debug)";
        public const string Label_SingleTask = "Single Task";
        public const string Label_RepeatingTask = "Repeating Task";
        public const string Label_HabitualTask = "Habitual Task";

        public const string PropertyName_Index = "Index";
        public const string PropertyName_Name = "Name";
        public const string PropertyName_TaskCount = "Tasks";
        public const string PropertyName_TaskIds = "TaskIds";
        public const string PropertyName_IncompleteCount = "Incomplete";

        public const string PropertyName_TaskType = "Type";
        public const string PropertyName_Description = "Description";
        public const string PropertyName_Notes = "Notes";
        public const string PropertyName_DueDate = "DueDate";
        public const string PropertyName_Completed = "Completed";
        public const string PropertyName_Overdue = "Overdue";
        public const string PropertyName_Interval = "Interval";
        public const string PropertyName_InFolderId = "InFolder Id";

        public const string PropertyName_PropertyName = "Property Name";
        public const string PropertyName_PropertyValue = "Property Value";

        public const string Command_Exit = "exit_taskmanager";
        public const string Command_Back = "go_back";

        public const string Command_AllFoldersView = "view_folders";
        public const string Command_ViewFolder = "view_a_folder";
        public const string Command_CreateFolder = "create_folder";
        public const string Command_DeleteFolder = "delete_folder";

        public const string Command_ViewTask = "view_task";
        public const string Command_CreateTask = "create_task";
        public const string Command_DeleteTask = "delete_task";

        public const string Command_EditTaskDescription = "edit_task_description";
        public const string Command_EditTaskNotes = "edit_task_notes";
        public const string Command_EditTaskDueDate = "edit_task_duedate";
        public const string Command_Complete_Task = "complete_task";
        public const string Command_Uncomplete_Task = "de-complete_task";
        public const string Command_Reload_Task = "reload_task";


        public static readonly Dictionary<string, string> MainMenu = new Dictionary<string, string>()
        {
            { Command_AllFoldersView, "View Task Folders" },
            { Command_Exit, "Exit Task Manager"},
        };

        public static readonly Dictionary<string, string> FoldersViewMenu = new Dictionary<string, string>()
        {
            { Command_ViewFolder, "View Folder" },
            { Command_CreateFolder, "Create Folder" },
            { Command_DeleteFolder, "Delete Folder" },
            { Command_Back, ""},
        };

        public static readonly Dictionary<string, string> TasksViewMenu = new Dictionary<string, string>()
        {
            { Command_ViewTask, "View Task" },
            { Command_CreateTask, "Create Task" },
            { Command_DeleteTask, "Delete Task" },
            { Command_Back, ""},
        };

        public static readonly Dictionary<string, string> TasksMenu = new Dictionary<string, string>()
        {
            { Command_Complete_Task, "Mark Completed" },
            { Command_Uncomplete_Task, "Mark Incompleted" },
            { Command_EditTaskDescription, "Edit Description" },
            { Command_EditTaskNotes, "Edit Notes" },
            { Command_EditTaskDueDate, "Edit Due Date" },
            { Command_DeleteTask, "Delete Task" },
            { Command_Reload_Task, "" },
            { Command_Back, ""},
        };

        public static Dictionary<string, int> FoldersColumnLayout = new Dictionary<string, int>() {
            { PropertyName_Index, 8 },
            { PropertyName_Name, 30 },
            { PropertyName_TaskCount, 15 },
            { PropertyName_IncompleteCount, 15 },
        };

        public static Dictionary<string, int> TasksColumnLayout = new Dictionary<string, int>() {
            { PropertyName_Index, 8 },
            { PropertyName_Completed, 10 },
            { PropertyName_Description, 30 },
            { PropertyName_TaskType, 15 },
            { PropertyName_Notes, 50 },
            { PropertyName_DueDate, 25 },
            { PropertyName_Overdue, 3 },
        };

        public static Dictionary<string, int> TaskPropertiesColumnLayout = new Dictionary<string, int>() {
            { PropertyName_PropertyName, 15 },
            { PropertyName_PropertyValue, 75 },
        };
    }
}
