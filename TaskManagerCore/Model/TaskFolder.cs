using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Linq;

namespace TaskManagerCore.Model
{
    /// <summary>
    /// Represents a folder for organizing tasks.
    /// </summary>
    public class TaskFolder
    {
        /// <value>
        /// The unique identifier of the folder.
        /// </value>
        public string Id { get; }
        /// <value>
        /// The name of the folder.
        /// </value>
        public string Name { get; }

        private List<string> _taskIds;
        /// <value>
        /// The list of task IDs associated with the folder.
        /// </value>
        public List<string> TaskIds // Uni-directional one-to-many relationship
        { 
            get => new List<string>(_taskIds);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskFolder"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the folder.</param>
        public TaskFolder(string name)
        {
            Id = ""; // if TaskFolder is being instantiated this way, Id can be empty
            Name = name;
            _taskIds = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskFolder"/> class with the specified parameters.
        /// </summary>
        /// <param name="id">The unique identifier of the folder.</param>
        /// <param name="name">The name of the folder.</param>
        /// <param name="taskIds">The list of task IDs associated with the folder.</param>
        public TaskFolder(string id, string name, List<string> taskIds)
            : this(name)
        {
            Id = id;
            _taskIds = taskIds;
        }

        #region Builder methods
        /// <summary>
        /// Returns a new instance of <see cref="TaskFolder"/> with the specified name.
        /// </summary>
        /// <param name="value">The new name for the folder.</param>
        /// <returns>A new <see cref="TaskFolder"/> instance with the updated name.</returns>
        public TaskFolder WithName(string value)
        {
            //value ??= string.Empty; // compound assignment
            if (value == null)
            {
                value = string.Empty;
            }
            return new TaskFolder(Id, value, TaskIds);
        }

        /// <summary>
        /// Returns a new instance of <see cref="TaskFolder"/> with the specified task added.
        /// </summary>
        /// <param name="taskId">The ID of the task to add.</param>
        /// <returns>A new <see cref="TaskFolder"/> instance with the task added.</returns>
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

        /// <summary>
        /// Returns a new instance of <see cref="TaskFolder"/> with the specified task removed.
        /// </summary>
        /// <param name="taskId">The ID of the task to remove.</param>
        /// <returns>A new <see cref="TaskFolder"/> instance with the task removed.</returns>
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

        #region Equals and GetHashCode
        public override bool Equals(object obj)
        {
            return Equals(obj as TaskFolder);
        }

        public bool Equals(TaskFolder other)
        {
            if (other == null)
                return false;

            return Id == other.Id &&
                   Name == other.Name &&
                   _taskIds.SequenceEqual(other._taskIds);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17; // Start with small prime, convention
                hash = hash * 23 + (Id?.GetHashCode() ?? 0);
                hash = hash * 23 + (Name?.GetHashCode() ?? 0);
                foreach (var taskId in _taskIds)
                {
                    hash = hash * 23 + (taskId?.GetHashCode() ?? 0);
                }
                return hash;
            }
        }
        #endregion

    }
}
