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
            throw new NotImplementedException();
        }
    }

    class MyTaskDataFileReader : BinaryFileReader<TaskDataEntity>
    {

        public MyTaskDataFileReader(BinaryFileConfig config) : base(config) { }

        /// <summary>
        /// Reads current TaskDataEntity with provided BinaryReader instance
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected override TaskDataEntity ReadObject(BinaryReader reader)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
