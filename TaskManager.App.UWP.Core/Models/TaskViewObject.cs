using System;
using System.Collections.Generic;
using System.Text;
using TaskManagerCore.Model;

namespace TaskManager.App.UWP.Core.Models
{
    public class TaskViewObject
    {
        public TaskType Type { get; set; }
        public string GlobalId { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Completed { get; set; }
        public bool Overdue { get; set; }
        public string ParentFolderName { get; set; }
        public int Streak { get; set; }

        public TaskViewObject()
        {
            Description = "";
            Notes = "";
            ParentFolderName = "";
        }

        public string ShortDescription => Description.Length > 10 ? $"{Description.Substring(0, 10)}..." : $"{Description}";
    }
}
