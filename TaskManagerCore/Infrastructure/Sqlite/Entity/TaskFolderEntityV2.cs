using System.ComponentModel.DataAnnotations;
using TaskManagerCore.Configuration;

namespace TaskManagerCore.Infrastructure.Sqlite.Entity
{
    internal class TaskFolderEntityV2 : EntityBaseV2
    {
        public string Name { get; set; }
        //public List<string> TaskIds { get; set; } // Uni-directional one-to-many relationship

        public List<string> TaskIds { get; set; }
        public List<TaskDataEntityV2> Tasks { get; set; }

        public TaskFolderEntityV2()
            :this ("")
        {
        }

        public TaskFolderEntityV2(string? id = "")
            : base(id)
        {
            Name = "";
            TaskIds = new List<string>();
            Tasks = new List<TaskDataEntityV2>();
        }

        #region Static Helper Methods
        public static TaskFolderEntityV2 BLANK => new TaskFolderEntityV2();
        #endregion

        public override string ToString()
        {
            return $"TaskFolderEntity: [ID={GlobalId}, Name={Name}, Tasks Count={TaskIds.Count}]";
        }
    }
}
