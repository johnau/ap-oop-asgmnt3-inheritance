using TaskManagerCore.Model;

namespace TaskManagerCore.XunitTests.TestHelpers
{
    /// <summary>
    /// This helper class is effecitvely just a wrapper around the tested class to bridge the assembly boundary.
    /// C# Does not allow casting across assembly boundaries
    /// </summary>
    public class RepeatingTaskDataTestHelperExtension : RepeatingTaskData
    {
        public DateTime FakeDateTime {  get; set; }
        

        public RepeatingTaskDataTestHelperExtension(string description, string notes, DateTime dueDate, TimeInterval interval) 
            : base(description, notes, dueDate, interval)
        {
            FakeDateTime = DateTime.Now;
        }

        /// <summary>
        /// RepeatingTaskData Constructor Wrapper (for WithCompleted method)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="notes"></param>
        /// <param name="completed"></param>
        /// <param name="dueDate"></param>
        /// <param name="interval"></param>
        /// <param name="repititions"></param>
        public RepeatingTaskDataTestHelperExtension(string id, string description, string notes, bool completed, DateTime dueDate, TimeInterval interval, int repititions)
            : base(id, description, notes, completed, dueDate, interval, repititions)
        {
            FakeDateTime = DateTime.MinValue;
        }

        /// <summary>
        /// Convenience method to set a specific date
        /// </summary>
        /// <param name="dateString"></param>
        public void SetFakeDateTimeByString(string dateString)
        {
            FakeDateTime = DateTime.Parse(dateString);
        }

        /// <summary>
        /// WithCompleted Method Wrapper
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override RepeatingTaskDataTestHelperExtension WithCompleted(bool value)
        {
            var completed = base.WithCompleted(value); 
            // process the base class method and assign values here - this isn't the nicest, but interfaces don't feel appropriate.
            // The TaskData classes could be refactored as structs - lightweight and free of functionality, with accessor classes for the business logic
            // would make nicer testing?
            return new RepeatingTaskDataTestHelperExtension(completed.Id, completed.Description, completed.Notes, false, completed.DueDate, completed.RepeatingInterval, completed.Repititions)
            {
                FakeDateTime = FakeDateTime
            };
        }

        protected override DateTime ComparisonTime()
        {
            return FakeDateTime;
        }
    }
}
