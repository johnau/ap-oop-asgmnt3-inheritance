using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerCore.Infrastructure.Sqlite.Entity
{
    internal class TaskDataEntityV2 : EntityBaseV2
    {
        public string Description { get; set; }

        public string Notes { get; set; }

        public bool Completed { get; set; }

        private DateTime? _dueDate;
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                _dueDate = value;
            }
        }

        public long RepeatingInterval {  get; set; }
        public long Repetitions { get; set; }
        public long Streak { get; set; }

        public TaskFolderEntityV2 ParentFolder { get; set; }

        public TaskDataEntityV2()
            : this("")
        { }

        /// <summary>
        /// No args constructor (allows setting id to avoid creating a DTO object that takes id)
        /// Setting the Id is for use when entity is acting as a DTO to the real entity held in the 'persistence context'
        /// </summary>
        /// <param name="globalId"></param>
        public TaskDataEntityV2(string globalId = "")
            : base(globalId)
        {
            Description = "";
            Notes = "";
            Completed = false;
            RepeatingInterval = -1;
            Repetitions = -1;
            Streak = -1;
        }

        #region Static Helper Methods
        public static TaskDataEntityV2 BLANK => new TaskDataEntityV2();
        #endregion

        public override string ToString()
        {
            return $"TaskDataEntity: [ID={GlobalId}, Description={Description}, Notes={Notes}, Completed={Completed}, DueDate={DueDate}, RepeatingInterval={RepeatingInterval}, Repetitions={Repetitions}, Streak={Streak}]";
        }
    }
}
