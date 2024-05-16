using BinaryFileHandler;
using System.ComponentModel.DataAnnotations;
using TaskManagerCore.Configuration;

namespace TaskManagerCore.Infrastructure.Sqlite.Entity
{
    internal class TaskDataEntity : EntityBase
    {
        [Key]
        public long TaskDataEntityId { get; set; }
        public string Description { get; set; }

        public string Notes { get; set; }

        public bool Completed { get; set; }

        private DateTime? _dueDate;
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                if (value.HasValue && value.Value > DateTime.MinValue)
                    _dueDate = value.Value;
                else
                    _dueDate = null;
            }
        }

        public long RepeatingInterval {  get; set; }
        public long Repetitions { get; set; }
        public long Streak { get; set; }

        public TaskDataEntity()
            : this("")
        {
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
            RepeatingInterval = -1;
            Repetitions = -1;
            Streak = -1;
        }

        #region Static Helper Methods
        public static TaskDataEntity BLANK => new TaskDataEntity();
        #endregion

        public override string ToString()
        {
            return $"TaskDataEntity: [ID={Id}, Description={Description}, Notes={Notes}, Completed={Completed}, DueDate={DueDate}, RepeatingInterval={RepeatingInterval}, Repetitions={Repetitions}, Streak={Streak}]";
        }
    }
}
