using BinaryFileHandler;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
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

            var conf = new BinaryFileConfig("testing-task-data");
            var writer = new TaskDataFileWriter(conf);
            writer.AddObjectToWrite(task1);
            writer.AddObjectToWrite(task2);
            var success = writer.WriteValues();

            Assert.True(File.Exists(writer.FilePath));

            Debug.WriteLine($"File written to: {writer.FilePath}");
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
                Repetitions = 5,
            };            
            var task4 = new HabitualTaskDataEntity(Guid.NewGuid().ToString())
            {
                Description = "Test Description 4",
                Notes = "Test Notes 4",
                Completed = false,
                DueDate = DateTime.Now.AddDays(4),
                RepeatingInterval = TimeInterval.Weekly,
                Repetitions = 5,
                Streak = 2,
            };

            var conf = new BinaryFileConfig("testing-task-data2");
            var writer = new TaskDataFileWriter(conf);
            writer.AddObjectToWrite(task1);
            writer.AddObjectToWrite(task2);
            writer.AddObjectToWrite(task3);
            writer.AddObjectToWrite(task4);
            var success = writer.WriteValues();

            Assert.True(File.Exists(writer.FilePath));

            Debug.WriteLine($"File written to: {writer.FilePath}");
        }
    }
}
