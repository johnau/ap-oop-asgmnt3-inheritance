# TaskManager Documentation

The Task Manager is a multi-part assignment for North Metro TAFE C# OOP Unit, part of the Diploma of ICT - Advanced Programming.
This documentation is part of Part 4 of the assessment; documentating usage of the code so far.

These docs have been created using [DocFX](https://dotnet.github.io/docfx/index.html) to generate a static website.
An alternate set of docs have been created using [Sandcastle](https://ewsoftware.github.io/SHFB/html/bd1ddb51-1c4f-434f-bb1a-ce2135d3a909.htm), however the results of docFx were preferable.
An attempt to manually write docs in Markdown using [Obsidian](https://obsidian.md/), however this was abandoned due to the inability to keep in sync with the codebase.

> [!TIP]
>
> **TLDR;**
>
> This project was created for an Assignment

## Overview of Docs

- [General Documentation](./general/README.md)
  - [Architectural Decisions](./general/architecture-decisions/README.md) - *Decisions regarding the design of the application, including decision log.*
  - [Getting Started](./general/getting-started/README.md) - *How to use the Task Manager*
  - [Documentation Guidelines](./general/guidelines/README.md) - *How to add to these docs*
- [Service Documentation](./services/README.md) - *Task Manager Core services*
  - [TaskController Documentation](./services/TaskController/README.md) - *API service*
  - [TaskManagerCore Reference](xref:TaskManagerCore) - *Code reference*

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
      TaskManager.CreateTaskFolder(new CreateFolderDto("Work")); // 'work' folder
      TaskManager.CreateTaskFolder(new CreateFolderDto("School")); // 'school' folder

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

      // Move a Task (eg. from 'work' to 'school')
      var success1 = TaskManager.MoveTask(taskId, "work", "school");

      // Complete a task
      var success2 = TaskManager.CompleteTask(taskId);

      // Check incomplete task count
      var incompleteCount = TaskManager.CountIncompleteInFolder("school");
    }
  }
}
```

## Project structure

```xaml
📂TaskManager // root
  📂BinaryFileHandler // for Assessment Part 2 - Binary File Handling
  📂docs // documentation root
    📂_site // documentation site
      📄These Docs
  📂InMemoryCache // for Assessment Part 3 - Indexing
  📂InMemoryCache.XunitTests // for testing Part 3
  📂TaskManagerConsoleApp // for Assessment Part 1 - Console output
  📂TaskManagerCore // Application core
  📂TaskManagerCore.XunitTests // tests for application core
  📄TaskManager.sln // Visual Studio Solution file
  📄readme.md // original Readme file
```