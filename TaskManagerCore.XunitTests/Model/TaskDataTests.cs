using TaskManagerCore.XunitTests.TestHelpers;

namespace TaskManagerCore.XunitTests.Model
{
    public class TaskDataTests
    {

        [Fact]
        public void Task_WithDueDate_TestBehavior()
        {
            var taskData = new TaskDataTestHelperExtension(Guid.NewGuid().ToString(), "", "", false, DateTime.Now.AddHours(1));
            taskData.FakeDateTime = DateTime.Now;
            
            Assert.False(taskData.Overdue);

            var description = "Test Task Description";
            taskData = taskData.WithDescription(description);
            
            Assert.Equal(description, taskData.Description);

            var notes = "Test Task Notes";
            taskData = taskData.WithNotes(notes);

            Assert.Equal(notes, taskData.Notes);

            taskData.FakeDateTime = DateTime.Now.AddHours(2);

            Assert.True(taskData.Overdue);

            taskData = taskData.WithCompleted(true);

            Assert.False(taskData.Overdue);
            Assert.True(taskData.Completed);
        }

        [Fact]
        public void Task_WithDueDate_TestBehavior2()
        {
            var taskData = new TaskDataTestHelperExtension(Guid.NewGuid().ToString(), "", "", false, null);
            taskData.FakeDateTime = DateTime.Now;

            Assert.False(taskData.Overdue);

            taskData.FakeDateTime = DateTime.Now.AddHours(9999);

            Assert.False(taskData.Overdue);

            var dueDate = DateTime.Now.AddHours(1);
            taskData = taskData.WithDueDate(dueDate);

            Assert.Equal(dueDate, taskData.DueDate);

            Assert.True(taskData.Overdue);

            taskData = taskData.WithCompleted(true);

            Assert.False(taskData.Overdue);
            Assert.True(taskData.Completed);
        }
    }
}
