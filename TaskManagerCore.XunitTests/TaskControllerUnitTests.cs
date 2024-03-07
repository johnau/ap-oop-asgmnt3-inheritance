using System.Threading.Tasks;
using TaskManagerCore.Configuration;
using TaskManagerCore.Controller;
using TaskManagerCore.Model;
using TaskManagerCore.Model.Dto;
using TaskManagerCore.XunitTests.TestHelpers;

namespace TaskManagerCore.XunitTests
{
    public class TaskControllerUnitTests
    {
        public ICrudRepository<TaskData, string> TaskDataRepository;
        public ICrudRepository<TaskFolder, string> TaskFolderRepository;

        public TaskControllerUnitTests()
        {
            TaskDataRepository = new MockTaskDataRepository();
            TaskFolderRepository = new MockTaskFolderRepository();
        }

        [Fact]
        public void GetTasks_WillSucceed()
        {
            var taskDatas = new List<TaskData>()
            {
                new TaskData("Test Task 1 - Do The Thing"),
                new RepeatingTaskData("Test Task 2 - Hourly Task", "", DateTime.Now.AddHours(1), TimeInterval.Hourly),
                new HabitualTaskData("Test Task 3 - Daily Exercise", "", DateTime.Now.AddHours(1), TimeInterval.Daily)
            };

            // Create mock instances of classes
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindAll = () => taskDatas,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);

            var tasks = controller.GetTasks();

            Assert.Equal(3, tasks.Count);
        }

        /// <summary>
        /// This test also tests the DTO's (probably sufficient)
        /// </summary>
        [Fact]
        public void GetTaskById_ValidId_WillSucceed()
        {
            var taskDescription = "Test Task 1 - Do The Thing";
            var taskNotes = "";
            var due = DateTime.Now.AddHours(1);
            var taskData = new TaskData(Guid.NewGuid().ToString(), taskDescription, taskNotes, false, due);
            var taskDataId = taskData.Id;

            // Create mock instances of classes
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => id == taskDataId ? taskData : null,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var task = controller.GetTaskById(taskDataId);

            Assert.NotNull(task);
            Assert.Equal(taskDescription, task.Description);
            Assert.Equal(taskNotes, task.Notes);
            Assert.Equal(due, task.DueDate);
            Assert.False(task.Overdue);
            Assert.False(task.Completed);
            Assert.Equal(taskDataId, task.Id);
            Assert.Equal(TaskType.SINGLE, task.Type);

        }

        [Fact]
        public void GetTaskById_ValidId_WillFail()
        {
            var taskData = new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, DateTime.Now.AddHours(1));
            var taskDataId = taskData.Id;

            // Create mock instances of classes
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => id == taskDataId ? taskData : null,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var task = controller.GetTaskById("bad task id");

            Assert.Null(task);
        }

        [Fact]
        public void GetTaskFolders_WillSucceed()
        {
            var taskFolders = new List<TaskFolder>()
            {
                new TaskFolder("Test folder 1"),
                new TaskFolder("Test folder 2"),
                new TaskFolder("Test folder 3"),
            };

            // Create mock instances of classes
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindAll = () => taskFolders,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var folders = controller.GetTaskFolders();

            Assert.Equal(3, folders.Count);
        }

        [Fact]
        public void GetTaskFolderById_ValidId_WillSucceed()
        {
            var taskFolder = new TaskFolder(Guid.NewGuid().ToString(), "Test Folder 1", new List<string>());
            var taskFolderId = taskFolder.Id;

            // Create mock instances of classes
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => id == taskFolderId ? taskFolder : null,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var folder = controller.GetTaskFolderById(taskFolderId);

            Assert.NotNull(folder);
        }

        [Fact]
        public void GetTaskFolderById_ValidId_WillFail()
        {
            var taskFolder = new TaskFolder(Guid.NewGuid().ToString(), "Test Folder 1", new List<string>());
            var taskFolderId = taskFolder.Id;

            // Create mock instances of classes
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => id == taskFolderId ? taskFolder : null,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var folder = controller.GetTaskFolderById("bad id");

            Assert.Null(folder);
        }

        [Fact]
        public void CountIncomplete_WithExistingFolder_ThreeIncomplete()
        {
            // Create mock instances of classes

            var taskDatas = new List<TaskData>()
            {
                new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, null),
                new RepeatingTaskData(Guid.NewGuid().ToString(), "Test Task 2 - Hourly Task", "", false, DateTime.Now.AddHours(1), TimeInterval.Hourly, 0),
                new HabitualTaskData(Guid.NewGuid().ToString(), "Test Task 3 - Daily Exercise", "", false, DateTime.Now.AddHours(1), TimeInterval.Daily, 0, 0)
            };

            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindByIds = (idList) => taskDatas,
            };

            var taskFolder = new TaskFolder(Guid.NewGuid().ToString(), "Test Folder 1", taskDatas.Select(task => task.Id).ToList());
            var taskFolderId = taskFolder.Id;

            // Create mock instances of classes
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => id == taskFolderId ? taskFolder : null,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var count = controller.CountIncomplete(taskFolderId);

            Assert.Equal(3, count);
        }

        [Fact]
        public void CountIncomplete_WithExistingFolder_ZeroIncomplete()
        {
            // Create mock instances of classes
            var taskDatas = new List<TaskData>()
            {
                new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", true, null),
                new RepeatingTaskData(Guid.NewGuid().ToString(), "Test Task 2 - Hourly Task", "", true, DateTime.Now.AddHours(1), TimeInterval.Hourly, 0),
                new HabitualTaskData(Guid.NewGuid().ToString(), "Test Task 3 - Daily Exercise", "", true, DateTime.Now.AddHours(1), TimeInterval.Daily, 0, 0)
            };

            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindByIds = (idList) => taskDatas,
            };

            var taskFolder = new TaskFolder(Guid.NewGuid().ToString(), "Test Folder 1", taskDatas.Select(task => task.Id).ToList());
            var taskFolderId = taskFolder.Id;

            // Create mock instances of classes
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => id == taskFolderId ? taskFolder : null,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var count = controller.CountIncomplete(taskFolderId);

            Assert.Equal(0, count);
        }

        [Fact]
        public void CountIncomplete_WithMissingFolder_WillFail()
        {
            // Test setup: Create mock instances of classes
            TaskDataRepository = new MockTaskDataRepository();
            var taskFolder = new TaskFolder("not used", "", new());
            var taskFolderId = taskFolder.Id;
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => id == taskFolderId ? taskFolder : null,
            };

            // Test
            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            Assert.Throws<Exception>(() => controller.CountIncomplete("bad id"));
        }

        [Fact]
        public void CompleteTask_WithValidId_WillSucceed()
        {
            var task = new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, null);
            var taskId = task.Id;
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => task,
                OnSave = (task) => taskId,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var savedTask = controller.CompleteTask(taskId);

            Assert.True(savedTask);
        }

        [Fact]
        public void CompleteTask_WithInvalidId_WillFail()
        {
            var task = new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, null);
            var taskId = task.Id;
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => id == taskId ? task : null,
                OnSave = (task) => taskId,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);

            Assert.Throws<Exception>(() => controller.CompleteTask("bad id"));
        }

        [Fact]
        public void CompleteTask_WithValidIdButBadRepository_WillFail()
        {
            var task = new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, null);
            var taskId = task.Id;
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => task,
                OnSave = (task) => null,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);

            var result = controller.CompleteTask(taskId);

            Assert.False(result);
        }

        [Fact]
        public void DeleteTaskFromFolder_ValidFolderIdAndValidTaskId_WillSuceed()
        {
            var task = new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, null);
            var taskId = task.Id;
            var taskFolder = new TaskFolder("not used", "", new());
            var taskFolderId = taskFolder.Id;

            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => id == taskId ? task : null,
                OnDelete = (id) => true,
            };
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => id == taskFolderId ? taskFolder : null,
                OnSave = (folder) => "",
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);

            var result = controller.DeleteTaskFromFolder(taskFolderId, taskId);

            Assert.True(result);
        }

        [Fact]
        public void DeleteTaskFromFolder_InvalidFolderId_WillFail()
        {
            var task = new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, null);
            var taskId = task.Id;
            var taskFolder = new TaskFolder("not used", "", new());
            var taskFolderId = taskFolder.Id;

            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => id == taskId ? task : null,
                OnDelete = (id) => true,
            };
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => id == taskFolderId ? taskFolder : null,
                OnSave = (folder) => "",
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);

            Assert.Throws<Exception>(() => controller.DeleteTaskFromFolder("bad folder id", taskId));
        }

        [Fact]
        public void DeleteTaskFromFolder_InvalidTaskId_WillFail()
        {
            var task = new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, null);
            var taskId = task.Id;
            var taskFolder = new TaskFolder("not used", "", new());
            var taskFolderId = taskFolder.Id;

            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => id == taskId ? task : null,
                OnDelete = (id) => true,
            };
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => id == taskFolderId ? taskFolder : null,
                OnSave = (folder) => "",
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);

            Assert.Throws<Exception>(() => controller.DeleteTaskFromFolder(taskFolderId, "bad task id"));
        }

        [Fact]
        public void DeleteTaskFromFolder_BadRepositoryTask_WillFail()
        {
            var task = new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, null);
            var taskId = task.Id;
            var taskFolder = new TaskFolder("not used", "", new());
            var taskFolderId = taskFolder.Id;

            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => id == taskId ? task : null,
                OnDelete = (id) => true,
            };
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => id == taskFolderId ? taskFolder : null,
                OnSave = (folder) => null,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);

            var result = controller.DeleteTaskFromFolder(taskFolderId, taskId);

            Assert.False(result);
        }

        [Fact]
        public void DeleteTaskFromFolder_BadRepositoryFolder_WillFail()
        {
            var task = new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, null);
            var taskId = task.Id;
            var taskFolder = new TaskFolder("not used", "", new());
            var taskFolderId = taskFolder.Id;

            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => id == taskId ? task : null,
                OnDelete = (id) => false,
            };
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => id == taskFolderId ? taskFolder : null,
                OnSave = (folder) => null,
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);

            var result = controller.DeleteTaskFromFolder(taskFolderId, taskId);

            Assert.False(result);
        }

        [Fact]
        public void CreateTask_ValidDto_WillSucceed()
        {
            // create dto
            var dto = new CreateTaskDto(TaskType.SINGLE, "folderId", "Test Task Description", "Notes", DateTime.Now.AddHours(1), TimeInterval.Daily);

            // fake Id that would be generated
            var mockId = Guid.NewGuid().ToString();

            // fake folder to be returned
            var mockFolder = new TaskFolder("test folder");

            // mock the response of TaskFolderRepository.FindById() to get the folder
            // mock the response of TaskDataRepository.Save() to generate the Task Id
            // mock the response of TaskFolderRepository.Save() to update the folder
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnSave = (task) => mockId,
            };
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => mockFolder,
                OnSave = (folder) => "test_does_not_matter",
            };

            // submit dto to the method and get newly created task id
            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var result = controller.CreateTask(dto);

            Assert.Equal(mockId, result);
        }

        [Fact]
        public void CreateTask_InvalidDtoBadFolderId_WillFail()
        {
            // create dto
            var dto = new CreateTaskDto(TaskType.SINGLE, "folderId", "Test Task Description", "Notes", DateTime.Now.AddHours(1), TimeInterval.Daily);

            // fake Id that would be generated
            var mockId = Guid.NewGuid().ToString();

            // fake folder to be returned
            var mockFolder = new TaskFolder("test folder");

            // mock the response of TaskFolderRepository.FindById() to get the folder
            // mock the response of TaskDataRepository.Save() to generate the Task Id
            // mock the response of TaskFolderRepository.Save() to update the folder
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnSave = (task) => mockId,
            };
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => null,
                OnSave = (folder) => "test_does_not_matter",
            };

            // submit dto to the method and get newly created task id
            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            Assert.Throws<Exception>(() => controller.CreateTask(dto));
        }

        [Fact]
        public void CreateTask_BadTaskRepo_WillFail()
        {
            // create dto
            var dto = new CreateTaskDto(TaskType.SINGLE, "folderId", "Test Task Description", "Notes", DateTime.Now.AddHours(1), TimeInterval.Daily);

            // fake Id that would be generated
            var mockId = Guid.NewGuid().ToString();

            // fake folder to be returned
            var mockFolder = new TaskFolder("test folder");

            // mock the response of TaskFolderRepository.FindById() to get the folder
            // mock the response of TaskDataRepository.Save() to generate the Task Id
            // mock the response of TaskFolderRepository.Save() to update the folder
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnSave = (task) => null,
            };
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => mockFolder,
                OnSave = (folder) => "test_does_not_matter",
            };

            // submit dto to the method and get newly created task id
            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            Assert.Throws<Exception>(() => controller.CreateTask(dto));
        }

        [Fact]
        public void CreateTask_BadFolderRepo_WillFail()
        {
            // create dto
            var dto = new CreateTaskDto(TaskType.SINGLE, "folderId", "Test Task Description", "Notes", DateTime.Now.AddHours(1), TimeInterval.Daily);

            // fake Id that would be generated
            var mockId = Guid.NewGuid().ToString();

            // fake folder to be returned
            var mockFolder = new TaskFolder("test folder");

            // mock the response of TaskFolderRepository.FindById() to get the folder
            // mock the response of TaskDataRepository.Save() to generate the Task Id
            // mock the response of TaskFolderRepository.Save() to update the folder
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnSave = (task) => null,
            };
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => null,
                OnSave = (folder) => "test_does_not_matter",
            };

            // submit dto to the method and get newly created task id
            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            Assert.Throws<Exception>(() => controller.CreateTask(dto));
        }

        [Fact]
        public void CreateTaskFolder_ValidDto_WillSucceed()
        {
            // create task folder dto
            var dto = new CreateFolderDto("Test Folder 1");
            var testId = Guid.NewGuid().ToString();

            // mock response of task folder save
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnSave = (folder) => testId,
            };

            // run test method
            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var result = controller.CreateTaskFolder(dto);

            Assert.Equal(testId, result);
        }

        [Fact]
        public void CreateTaskFolder_BadRepo_WillFail()
        {
            // create task folder dto
            var dto = new CreateFolderDto("Test Folder 1");
            var testId = Guid.NewGuid().ToString();

            // mock response of task folder save
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnSave = (folder) => throw new Exception("Test Fail"),
            };

            // run test method
            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            
            Assert.Throws<Exception>(() => controller.CreateTaskFolder(dto));
        }

        /// <summary>
        /// Not a great test of this method, more tests needed, but this method is not required or used and was just added becuase it seemed like it would be needed at some point.  
        /// Finish tests for this method if reuqired.
        /// </summary>
        /// <exception cref="Exception"></exception>
        [Fact]
        public void MoveTask_ValidIds_WillSucceed()
        {
            var task = new TaskData(Guid.NewGuid().ToString(), "Test Task 1 - Do The Thing", "", false, null);
            var taskId = task.Id;
            var taskFolderFrom = new TaskFolder(Guid.NewGuid().ToString(), "Folder From", new());
            var taskFolderFromId = taskFolderFrom.Id;            
            var taskFolderTo = new TaskFolder(Guid.NewGuid().ToString(), "Folder To", new());
            var taskFolderToId = taskFolderFrom.Id;

            // Set up mock responses
            TaskDataRepository = new MockTaskDataRepository()
            {
                OnFindById = (id) => id == taskId ? task : null,
                OnSave = (task) => taskId,
            };
            TaskFolderRepository = new MockTaskFolderRepository()
            {
                OnFindById = (id) => {
                    if (id == taskFolderFromId) return taskFolderFrom;
                    else if (id == taskFolderToId) return taskFolderTo;
                    else return null;
                },
                OnSave = (folder) =>
                {
                    if (folder.Id == taskFolderFrom.Id) return taskFolderFromId;
                    else if (folder.Id == taskFolderTo.Id) return taskFolderToId;
                    else throw new Exception("Test error");
                },
            };

            var controller = new TaskController(TaskDataRepository, TaskFolderRepository);
            var result = controller.MoveTask(taskId, taskFolderFromId, taskFolderToId);

            Assert.True(result);
        }
    }
}
