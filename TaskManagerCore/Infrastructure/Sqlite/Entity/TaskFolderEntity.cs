using System.ComponentModel.DataAnnotations;
using TaskManagerCore.Configuration;

namespace TaskManagerCore.Infrastructure.Sqlite.Entity
{
    internal class TaskFolderEntity : EntityBase
    {
        [Key]
        public long TaskFolderEntityId { get; set; } // for auto generated id in database (we will store a record for every change to keep track of changes)
        public string Name { get; set; }
        //public List<string> TaskIds { get; set; } // Uni-directional one-to-many relationship

        public List<TaskDataEntity> Tasks { get; set; }

        public TaskFolderEntity()
            :this ("")
        {

        }

        public TaskFolderEntity(string? id = "")
            : base(id)
        {
            Name = "";
            //TaskIds = new List<string>();
            Tasks = new List<TaskDataEntity>();
        }

        #region Static Helper Methods
        public static TaskFolderEntity BLANK => new TaskFolderEntity();
        #endregion

        public override string ToString()
        {
            return $"TaskFolderEntity: [ID={Id}, Name={Name}, Tasks Count={Tasks.Count}]";
        }
    }
}
