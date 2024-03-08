using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Model;

namespace TaskManagerCore.XunitTests.Infrastructure.Binary
{
    public class TaskDataFileWriterTests
    {
        [Fact]
        public void WriteValues_TwoSingleTasks_WillSucceed()
        {
            var task1 = new TaskDataEntity(Guid.NewGuid().ToString())
            {
                Description = "Test Description 1",
                Notes = "Test Notes 1",
                Completed = false,
            };
            var task2 = new TaskDataEntity(Guid.NewGuid().ToString())
            {
                Description = "Test Description 2",
                Notes = "Test Notes 2",
                Completed = true,
                DueDate = DateTime.Now.AddDays(1),
            };

            var writer = new TaskDataFileWriter("testing-task-data");
            writer.AddObjectToWrite(task1);
            writer.AddObjectToWrite(task2);
            var filePath = writer.WriteValues();

            Assert.True(File.Exists(filePath));

            Debug.WriteLine($"File written to: {filePath}");
        }

        [Fact]
        public void WriteValues_AllTaskTypes_WillSucceed()
        {
            var task1 = new TaskDataEntity(Guid.NewGuid().ToString())
            {
                Description = "Test Description 1",
                Notes = "Test Notes 1",
                Completed = false,
            };
            var task2 = new TaskDataEntity(Guid.NewGuid().ToString())
            {
                Description = "Test Description 2",
                Notes = "Test Notes 2",
                Completed = true,
                DueDate = DateTime.Now.AddDays(1),
            };
            var task3 = new RepeatingTaskDataEntity(Guid.NewGuid().ToString())
            {
                Description = "Test Description 3",
                Notes = "Test Notes 3",
                Completed = false,
                DueDate = DateTime.Now.AddDays(1),
                RepeatingInterval = TimeInterval.Daily,
                Repititions = 5,
            };            
            var task4 = new HabitualTaskDataEntity(Guid.NewGuid().ToString())
            {
                Description = "Test Description 4",
                Notes = "Test Notes 4",
                Completed = false,
                DueDate = DateTime.Now.AddDays(4),
                RepeatingInterval = TimeInterval.Weekly,
                Repititions = 5,
                Streak = 2,
            };

            var writer = new TaskDataFileWriter("testing-task-data2");
            writer.AddObjectToWrite(task1);
            writer.AddObjectToWrite(task2);
            writer.AddObjectToWrite(task3);
            writer.AddObjectToWrite(task4);
            var filePath = writer.WriteValues();

            Assert.True(File.Exists(filePath));

            Debug.WriteLine($"File written to: {filePath}");
        }
    }
}
