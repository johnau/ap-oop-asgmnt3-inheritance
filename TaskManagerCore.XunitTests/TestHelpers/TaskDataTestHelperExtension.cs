using TaskManagerCore.Model;

namespace TaskManagerCore.XunitTests.TestHelpers
{
    public class TaskDataTestHelperExtension : TaskData
    {
        public DateTime FakeDateTime { get; set; }

        public TaskDataTestHelperExtension(string id, string description, string notes, bool completed, DateTime? dueDate)
            : base(id, description, notes, completed, dueDate)
        { }

        /// <summary>
        /// All methods wrapped for this testing
        /// This has not resulted in the nicest test structure, leaving for now
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override TaskDataTestHelperExtension WithCompleted(bool value)
        {
            var _ = base.WithCompleted(value);
            return new TaskDataTestHelperExtension(_.Id, _.Description, _.Notes, _.Completed, _.DueDate)
            {
                FakeDateTime = FakeDateTime
            };
        }

        public override TaskDataTestHelperExtension WithDescription(string value)
        {
            var _ = base.WithDescription(value);
            return new TaskDataTestHelperExtension(_.Id, _.Description, _.Notes, _.Completed, _.DueDate)
            {
                FakeDateTime = FakeDateTime
            };
        }

        public override TaskDataTestHelperExtension WithNotes(string value)
        {
            var _ = base.WithNotes(value);
            return new TaskDataTestHelperExtension(_.Id, _.Description, _.Notes, _.Completed, _.DueDate)
            {
                FakeDateTime = FakeDateTime
            };
        }
        public override TaskDataTestHelperExtension WithDueDate(DateTime? value)
        {
            var _ = base.WithDueDate(value);
            return new TaskDataTestHelperExtension(_.Id, _.Description, _.Notes, _.Completed, _.DueDate)
            {
                FakeDateTime = FakeDateTime
            };
        }

        /// <summary>
        /// Convenience method to set a specific date
        /// </summary>
        /// <param name="dateString"></param>
        public void SetFakeDateTimeByString(string dateString)
        {
            FakeDateTime = DateTime.Parse(dateString);
        }

        protected override DateTime ComparisonTime()
        {
            return FakeDateTime;
        }
    }
}
