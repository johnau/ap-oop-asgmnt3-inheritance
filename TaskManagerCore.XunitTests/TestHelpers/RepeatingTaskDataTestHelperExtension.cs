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
