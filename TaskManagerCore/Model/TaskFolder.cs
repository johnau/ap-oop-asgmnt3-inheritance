using Microsoft.VisualBasic;

namespace TaskManagerCore.Model
{
    /// <summary>
    /// Immutable class (use 'record' after C# 9.0?) Or make this a struct and shift the logic to an accessor...
    /// 
    /// As per requirements:
    /// - The Id does not have a publicly accessible setter (populated at construction and then passed through builder methods internally)
    /// - Name, List of Task Id's
    /// - Overdue method
    /// </summary>
    public class TaskFolder
    {
        public string Id { get; }
        public string Name { get; }
        private List<string> _taskIds;
        public List<string> TaskIds // Uni-directional one-to-many relationship
        { 
            get => new List<string>(_taskIds);
        } 

        public TaskFolder(string name)
        {
            Id = ""; // if TaskFolder is being instantiated this way, Id can be empty
            Name = name;
            _taskIds = new List<string>();
        }

        public TaskFolder(string id, string name, List<string> taskIds)
            : this(name)
        {
            Id = id;
            _taskIds = taskIds;
        }

        #region Builder methods
        public TaskFolder WithName(string value)
        {
            //value ??= string.Empty; // compound assignment
            if (value == null)
            {
                value = string.Empty;
            }
            return new TaskFolder(Id, value, TaskIds);
        }

        public TaskFolder WithTask(string taskId)
        {
            if (TaskIds.Contains(taskId))
            {
                return this;
            }

            var _taskIds = new List<string>();
            _taskIds.AddRange(TaskIds);
            _taskIds.Add(taskId);
            return new TaskFolder(Id, Name, _taskIds);
        }

        public TaskFolder WithoutTask(string taskId)
        {
            if (!TaskIds.Contains(taskId))
            {
                return this;
            }

            var _taskIds = new List<string>(TaskIds);
            _taskIds.Remove(taskId);
            return new TaskFolder(Id, Name, _taskIds);
        }
        #endregion
    }
}
