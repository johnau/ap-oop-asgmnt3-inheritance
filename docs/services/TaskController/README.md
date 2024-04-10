# TaskController

The [TaskController class](xref:TaskManagerCore.Controller.TaskController) is used to interact with Tasks and Folders in the system.

An instance of the controller can be obtained from the [TaskManagerFactory](xref:TaskManagerCore.TaskManagerFactory) with the [CreateDefaultTaskManager() static method](xref:TaskManagerCore.TaskManagerFactory.CreateDefaultTaskManager).

The controller accepts and returns Data Transfer Objects (DTOs) to separate front-end from back-end.  Representing the Model data in the DTOs allows for flexibility of either layer without affecting the other.  
The following DTOs are used with the controller:
- [CreateFolder](xref:TaskManagerCore.Model.Dto.CreateFolderDto)
- [CreateTask](xref:TaskManagerCore.Model.Dto.CreateTaskDto)
- [GetFolder](xref:TaskManagerCore.Model.Dto.GetFolderDto)
- [GetTask](xref:TaskManagerCore.Model.Dto.GetTaskDto)

If required, the persistence layer can be replaced (eg. with a SQLite database). Two interfaces need to be implemented and provided to the <c>TaskController</c> constructor:
- [ITaskDataRepository](xref:TaskManagerCore.Model.Repository.ITaskDataRepository)
- [ITaskFolderRepository](xref:TaskManagerCore.Model.Repository.ITaskFolderRepository)