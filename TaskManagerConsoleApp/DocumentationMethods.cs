using TaskManagerCore;
using TaskManagerCore.Controller;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Dto;

namespace TaskManagerConsoleApp {
    /// <summary>
    /// The following class represents simple functionality of the TaskController
    /// </summary>
    class TaskManagerSample
    {
        private TaskController taskController = 
                            TaskManagerFactory.CreateDefaultTaskManager();

        public void BasicFolderActions()
        {
            var folder = new CreateFolderDto("Work");

            string id = taskController.CreateTaskFolder(folder);

            GetFolderDto? folderByName = taskController.GetTaskFolder("work");

            GetFolderDto? folderById = taskController.GetTaskFolderById(id);

            bool success = taskController.DeleteTaskFolder(folderByName!.Name);
        }

        public void BasicTaskActions()
        {
            var task = new CreateTaskDto(TaskType.SINGLE, 
                                        "work", // case-insensitive
                                        "A new task", 
                                        "This task is for testing purposes", 
                                        DateTime.Now.AddDays(1)
                                        );

            string id = taskController.CreateTask(task);
            
            taskController.CompleteTask(id);
            
            taskController.DeleteTaskFromFolderById("work", id);
        }

        public void ListTasksAndFolders()
        {
            // Get all folders from the controller
            List<GetFolderDto> allFolders = taskController.GetTaskFolders();


        }
    }

}
