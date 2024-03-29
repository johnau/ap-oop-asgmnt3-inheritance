using BinaryFileHandler;
using TaskManagerCore.Configuration;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class TaskDataEntity : EntityBase, IComparable<TaskDataEntity>, ISearchable, IBinaryWritableReadable<TaskDataEntity>
    {
        public string Description { get; set; }

        public string Notes { get; set; }

        public bool Completed { get; set; }

        private DateTime? _dueDate;
        public DateTime? DueDate {
            get { return _dueDate; }
            set 
            { 
                if (value.HasValue && value.Value > DateTime.MinValue)
                    _dueDate = value.Value;
                else
                    _dueDate = null;
            }
        }

        /// <summary>
        /// No args constructor (allows setting id to avoid creating a DTO object that takes id)
        /// Setting the Id is for use when entity is acting as a DTO to the real entity held in the 'persistence context'
        /// </summary>
        /// <param name="id"></param>
        public TaskDataEntity(string? id = "")
            : base(id)
        {
            Description = "";
            Notes = "";
            Completed = false;
        }

        public virtual int CompareTo(TaskDataEntity? other)
        {
            if (other == null) return 1;

            var compareCompleted = Completed.CompareTo(other.Completed);
            if (compareCompleted != 0) return compareCompleted;

            var compareDescription = string.Compare(Description, other.Description, StringComparison.OrdinalIgnoreCase);
            if (compareDescription != 0) return compareDescription;

            // This is ugly but is to protect against DueDate being populated with a DateTime(0) object instead of null
            // ... Have now added protection to the setter, but will leave this here for now.
            if (DueDate.HasValue && DueDate.Value > DateTime.MinValue
                && other.DueDate.HasValue && other.DueDate.Value > DateTime.MinValue)
            {
                var compareDueDate = DueDate.Value.CompareTo(other.DueDate.Value);
                if (compareDueDate != 0) return compareDueDate;
            }
            else if (DueDate.HasValue && DueDate.Value > DateTime.MinValue) return 1;
            else if (other.DueDate.HasValue && other.DueDate.Value > DateTime.MinValue) return -1;
            else return 0;

            return string.Compare(Notes, other.Notes, StringComparison.OrdinalIgnoreCase);
        }

        public string ToValuesOnlyString()
        {
            return $"{Description} {Notes} {Completed} {DueDate?.ToString("yyyy-MM dd_HH:mm tt")}";// use spaces for delimiters here for regex search matching?
        }

        #region Static Compare Methods
        public static int CompareByDueDate(TaskDataEntity x, TaskDataEntity y)
        {
            if (x.DueDate.HasValue && y.DueDate.HasValue)
            {
                return x.DueDate.Value.CompareTo(y.DueDate.Value);
            }
            else if (x.DueDate.HasValue) return 1;
            else if (y.DueDate.HasValue) return -1;

            return 0;
        }
        public static int CompareByCompleted(TaskDataEntity x, TaskDataEntity y)
        {
            return x.Completed.CompareTo(y.Completed);
        }

        public static int CompareByDescription(TaskDataEntity x, TaskDataEntity y)
        {
            return string.Compare(x.Description, y.Description, StringComparison.OrdinalIgnoreCase);
        }

        public static int CompareByNotes(TaskDataEntity x, TaskDataEntity y)
        {
            return string.Compare(x.Notes, y.Notes, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region Static Helper Methods
        public static TaskDataEntity BLANK => new TaskDataEntity();
        #endregion

        public override string ToString()
        {
            return $"TaskDataEntity: [ID={Id}, Description={Description}, Notes={Notes}, Completed={Completed}, DueDate={DueDate}]";
        }

        #region IBinaryWritableReadable interface methods
        /*
         * Not using the BinaryFile lib this way, have implemented
         */
        public virtual void WriteObject(BinaryWriter writer, TaskDataEntity obj)
        {
            throw new NotImplementedException();
        }

        public virtual TaskDataEntity ReadObject(BinaryReader reader, string className)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
