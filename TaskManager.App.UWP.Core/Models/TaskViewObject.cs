using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.App.UWP.Core.Models
{
    public class TaskViewObject
    {
        public string GlobalId { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public DateTime? DueDate { get; set; }
        public bool Completed { get; set; }
        public bool Overdue { get; set; }
        public string ParentFolderName { get; set; }

        public TaskViewObject()
        {
            Description = "";
            Notes = "";
            ParentFolderName = "";
        }

        public string ShortDescription => Description.Length > 10 ? $"Task: {Description.Substring(0, 10)}..." : $"Task: {Description}";
    }
}
