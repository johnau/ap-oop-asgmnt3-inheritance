# Getting Started with the TaskManagerCore

View the codebase at [TaskManagerCore @ Github](https://github.com/johnau/ap-oop-asgmnt3-inheritance/tree/part4). 

> [!TIP]
>
> Checkout the [TaskController](../../services/TaskController/README.md)

## Quick Start

The following C# code may be sufficient to get up and running with the TaskManagerCore.

```c#
using TaskManagerCore;
using TaskManagerCore.Controller;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Dto;

namespace MyApp {
  class TaskManagerSample
  {
    private readonly TaskController TaskManager =
                            TaskManagerFactory.CreateDefaultTaskManager();

    public void CreateFolderAndTask()
    {
      TaskManager.CreateTaskFolder(new CreateFolderDto("Work")); // Folder called 'work'
      TaskManager.CreateTaskFolder(new CreateFolderDto("School")); // Folder called 'school'

      var task = new CreateTaskDto(TaskType.SINGLE,
              "work", // folder created above (case-insensitive)
              "A new task",
              "",
              DateTime.Now.AddDays(1)
              );

      // Creating a Task
      var taskId = TaskManager.CreateTask(task);

      // Updating Task data
      var success = TaskManager.UpdateTaskProperty(taskId, "description", "Project Due");

      // Move a Task to another folder
      var success1 = TaskManager.MoveTask(taskId, "work", "school"); // from 'work' to 'school' folder

      // Complete a task
      var success2 = TaskManager.CompleteTask(taskId);

      // Check incomplete task count
      var incompleteCount = TaskManager.CountIncompleteInFolder("school");
    }
  }
}
```

## References

* [DocFX - static documentation generator](https://dotnet.github.io/docfx/index.html)
* [Providing quality documentation in your project with DocFx and Companion Tools](https://mtirion.medium.com/providing-quality-documentation-in-your-project-with-docfx-and-companion-tools-76aed42b1ddd)
