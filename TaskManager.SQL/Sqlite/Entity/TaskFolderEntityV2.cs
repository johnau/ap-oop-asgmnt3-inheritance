using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TaskManagerCore.SQL.Sqlite.Entity
{
    public class TaskFolderEntityV2 : EntityBaseV2
    {
        internal static readonly char separator = ';';

        public string Name { get; set; }
        
        // Keeping this to avoid changing the patterns from the other repository (to allow easy copy and paste of the logic)
        [NotMapped]
        public List<string> TaskIds
        {
            get
            {
                if (_taskIds == null)
                    _taskIds = new List<string>();

                return _taskIds;
            }
            set
            {
                _taskIds = value;
                _taskIdsString = string.Join(separator+"", _taskIds);
            }
        }
        private List<string> _taskIds;

        public List<TaskDataEntityV2> Tasks { get; set; }

        // Task Id's List as string for EF Core 2.0 (does not support list)
        public string TaskIdsString
        {
            get
            {
                if (TaskIds == null || TaskIds.Count == 0)
                    TaskIds = Tasks
                                .Select(t => t.GlobalId)
                                .ToList();

                _taskIdsString = string.Join(separator+"", TaskIds);
                return _taskIdsString;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    TaskIds = new List<string>();
                else
                    TaskIds = value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                                    .ToList();
            }
        }
        // Backing field for EF Core
        private string _taskIdsString;

        public TaskFolderEntityV2()
            :this ("")
        {
        }

        public TaskFolderEntityV2(string id = "")
            : base(id)
        {
            Name = "";
            TaskIds = new List<string>();
            Tasks = new List<TaskDataEntityV2>();
            _taskIdsString = "";
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
