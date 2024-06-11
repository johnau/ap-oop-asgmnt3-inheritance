using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaskManager.App.UWP.Core.Models
{
    public class TaskFolderViewObject
    {
        public string GlobalId { get; set; }
        public string Name { get; set; }
        public ICollection<TaskViewObject> Tasks { get; set; }
        public int TaskCount => Tasks.Count;
        public int IncompleteTaskCount => Tasks.Select(t => !t.Completed).Count();
        public int OverdueTaskCount => Tasks.Select(t => t.Overdue).Count();

        public char Symbol => (char)SymbolCode;

        public int SymbolCode { get; set; }

        public TaskFolderViewObject()
        {
            Name = "";
            Tasks = new List<TaskViewObject>();
            SymbolCode = 1;
        }

        public string ShortDescription => $"Folder: {Name}";
    }

}
