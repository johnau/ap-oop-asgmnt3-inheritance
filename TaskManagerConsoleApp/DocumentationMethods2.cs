using TaskManagerCore.Model;
using TaskManagerCore.Controller;
using TaskManagerCore.Infrastructure.BinaryFile;
using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using BinaryFileHandler;
using TaskManagerCore.Model.Repository;

namespace TaskManagerConsoleApp {
    /// <summary>
    /// The following class shows how to override default behavior
    /// </summary>
    class TaskManagerCustom2
    {
        public void CreateTaskManager() {
            var tasksFileConf = new BinaryFileConfig("taskmanager-task-data");
            var folderFileConf = new BinaryFileConfig("taskmanager-folder-data");

            // Implementat of BinaryFileWriter and BinaryFileReader to override data storage logic
            BinaryFileWriter<TaskDataEntity> taskWriter = new MyTaskDataFileWriter(tasksFileConf); 
            BinaryFileReader<TaskDataEntity> taskReader = new MyTaskDataFileReader(tasksFileConf);
            BinaryFileWriter<TaskFolderEntity> folderWriter = new MyTaskFolderFileWriter(folderFileConf);
            BinaryFileReader<TaskFolderEntity> folderReader = new MyTaskFolderFileReader(folderFileConf);
            
            var taskDao = new TaskDataDao(taskReader, taskWriter);
            var folderDao = new TaskFolderDao(folderReader, folderWriter);
            
            var taskRepository = new TaskDataRepository(taskDao);
            var folderRepository = new TaskFolderRepository(folderDao);
            
            // // Implement ITaskDataRepository and ITaskFolderDataRepository to
            // ITaskDataRepository taskRepository = new MyTaskRepository();
            // ITaskFolderRepository folderRepository = new MyFolderRepository();

            var controller = new TaskController(taskRepository, folderRepository);
        }
    }

    class MyTaskDataFileWriter : BinaryFileWriter<TaskDataEntity>
    {
        public MyTaskDataFileWriter(BinaryFileConfig config) : base(config) { }

        /// <summary>
        /// Writes current TaskDataEntity using provided BinaryWriter instance
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="entity"></param>
        protected override void WriteObject(BinaryWriter writer, TaskDataEntity entity)
        {
            writer.Write(entity.Id);
            writer.Write(entity.Description);
            writer.Write(entity.Notes);
            writer.Write(entity.Completed);
            writer.Write(entity.DueDate != null ? entity.DueDate.Value.Ticks : 0L);
            writer.Write(entity is RepeatingTaskDataEntity repeatingTask1 ? (int)repeatingTask1.RepeatingInterval : -1);
            writer.Write(entity is RepeatingTaskDataEntity repeatingTask2 ? repeatingTask2.Repetitions : -1);
            writer.Write(entity is HabitualTaskDataEntity habitualTask ? habitualTask.Streak : -1);
        }
    }

    class MyTaskDataFileReader : BinaryFileReader<TaskDataEntity>
    {
                private readonly List<string> acceptedClasses = new List<string>() {typeof(TaskDataEntity).Name,
                                                                            typeof(RepeatingTaskDataEntity).Name,
                                                                            typeof(HabitualTaskDataEntity).Name};

        public MyTaskDataFileReader(BinaryFileConfig config) : base(config) { }

        /// <summary>
        /// Reads current TaskDataEntity with provided BinaryReader instance
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected override TaskDataEntity ReadObject(BinaryReader reader)
        {
            if (!acceptedClasses.Contains(CurrentClassName))
                throw new ArgumentException("A recognized Class name was not detected");

            var id = reader.ReadString();
            var description = reader.ReadString();
            var notes = reader.ReadString();
            var completed = reader.ReadBoolean();
            var dueDate = reader.ReadInt64();
            var interval = reader.ReadInt32();
            var repetitions = reader.ReadInt32();
            var streak = reader.ReadInt32();

            return EntityFactory.TaskFromValues(CurrentClassName, 
                                                id, 
                                                description, 
                                                notes, 
                                                completed, 
                                                dueDate > 0L ? new DateTime(dueDate) : null, 
                                                (TimeInterval)interval, 
                                                repetitions, 
                                                streak);
        }
    }

    class MyTaskFolderFileWriter : BinaryFileWriter<TaskFolderEntity>
    {
       public MyTaskFolderFileWriter(BinaryFileConfig config) : base(config) { }

        /// <summary>
        /// Write current object
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="entity"></param>
        protected override void WriteObject(BinaryWriter writer, TaskFolderEntity entity)
        {
            writer.Write(entity.Id);
            writer.Write(entity.Name);
            writer.Write(string.Join(Delimiter, entity.TaskIds));
        }
    }

    class MyTaskFolderFileReader : BinaryFileReader<TaskFolderEntity>
    {
        public MyTaskFolderFileReader(BinaryFileConfig config) : base(config) { }

        /// <summary>
        /// Read current object
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected override TaskFolderEntity ReadObject(BinaryReader reader)
        {
            var id = reader.ReadString();
            var name = reader.ReadString();
            var taskIds = reader.ReadString().Split(Delimiter, StringSplitOptions.RemoveEmptyEntries);

            return new TaskFolderEntity(id)
            {
                Name = name,
                TaskIds = taskIds.ToList(),
            };
        }
    }

    class MyTaskRepository : ITaskDataRepository 
    {
        public MyTaskRepository()
        {
        }

        #region ICrudRepository methods
        public List<TaskData> FindAll()
        {
            throw new NotImplementedException();
        }

        public List<TaskData> FindByIds(List<string> ids)
        {
            throw new NotImplementedException();
        }

        public TaskData? FindById(string id)
        {
            throw new NotImplementedException();
        }

        public string Save(TaskData o)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region TaskData specific methods

        public List<TaskData> FindByDueDate(DateTime dueDate)
        {
            throw new NotImplementedException();
        }

        public List<TaskData> FindByDescription(string description)
        {
            throw new NotImplementedException();
        }

        //public TaskData? FindOneByDescription(string description)
        //{
        //    var result = Dao.FindOneByDescription(description);
        //    if (result == null) return null;
        //    return EntityFactory.ToModel(result);
        //}

        public List<TaskData> FindByNotes(string notes)
        {
            throw new NotImplementedException();
        }

        public List<TaskData> FindByCompleted(bool completed)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class MyFolderRepository : ITaskFolderRepository
    {
        internal MyFolderRepository()
        {
        }

        public List<TaskFolder> FindAll()
        {
            throw new NotImplementedException();
        }

        public List<TaskFolder> FindByIds(List<string> ids)
        {
            throw new NotImplementedException();
        }

        public TaskFolder? FindById(string id)
        {
            throw new NotImplementedException();
        }

        public string Save(TaskFolder o)
        {
            throw new NotImplementedException();
        }

        public bool Delete(string id)
        {
            throw new NotImplementedException();
        }

        #region TaskFolder specific methods
        public List<TaskFolder> FindByNameStartsWith(string name)
        {
            throw new NotImplementedException();
        }

        public TaskFolder? FindOneByName(string name)
        {
            throw new NotImplementedException();
        }

        public List<TaskFolder> FindEmpty()
        {
            throw new NotImplementedException();
        }

        public List<TaskFolder> FindNotEmpty()
        {
            throw new NotImplementedException();
        }

        public TaskFolder? FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public bool DeleteByName(string name)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
