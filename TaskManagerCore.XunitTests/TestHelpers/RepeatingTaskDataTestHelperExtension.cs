using TaskManagerCore.Model;

namespace TaskManagerCore.XunitTests.TestHelpers
{
    /// <summary>
    /// This project is about Inheritance, so have tried to use inheritance for testing
    /// </summary>
    public class RepeatingTaskDataTestHelperExtension : RepeatingTaskData
    {
        public DateTime FakeDateTime {  get; set; }
        

        public RepeatingTaskDataTestHelperExtension(string description, string notes, DateTime dueDate, TimeInterval interval) 
            : base(description, notes, dueDate, interval)
        {
            FakeDateTime = DateTime.Now;
        }

        public RepeatingTaskDataTestHelperExtension(string id, string description, string notes, bool completed, DateTime dueDate, TimeInterval interval, int repititions)
            : base(id, description, notes, completed, dueDate, interval, repititions)
        {
        }

        public void SetFakeDateTimeByString(string dateString)
        {
            FakeDateTime = DateTime.Parse(dateString);
        }

        public override RepeatingTaskDataTestHelperExtension WithCompleted(bool value)
        {
            var completed = base.WithCompleted(value); 
            // process the base class method and assign values here - this isn't the nicest, but interfaces don't feel appropriate.
            // really the model class should just be refactored to have the business logic shifted up the hierarchy, one level to an accessor class... would make testing easier
            return new RepeatingTaskDataTestHelperExtension(completed.Id, completed.Description, completed.Notes, false, completed.DueDate, completed.RepeatingInterval, completed.Repititions);
        }

        protected override DateTime ComparisonTime()
        {
            return FakeDateTime;
        }
    }
}
