﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TaskManagerCore.Model;

namespace TaskManagerCore.XunitTests.Infrastructure.Binary
{
    public class TaskDataFileReaderTests
    {
        TaskDataEntity ExampleTask1 = new TaskDataEntity(Guid.NewGuid().ToString())
        {
            Description = "Test Description 1",
            Notes = "Test Notes 1",
            Completed = false,
        };
        TaskDataEntity ExampleTask2 = new TaskDataEntity(Guid.NewGuid().ToString())
        {
            Description = "Test Description 2",
            Notes = "Test Notes 2",
            Completed = true,
            DueDate = DateTime.Now.AddDays(1),
        };
        RepeatingTaskDataEntity ExampleTask3 = new RepeatingTaskDataEntity(Guid.NewGuid().ToString())
        {
            Description = "Test Description 3",
            Notes = "Test Notes 3",
            Completed = false,
            DueDate = DateTime.Now.AddDays(1),
            RepeatingInterval = TimeInterval.Daily,
            Repititions = 5,
        };
        HabitualTaskDataEntity ExampleTask4 = new HabitualTaskDataEntity(Guid.NewGuid().ToString())
        {
            Description = "Test Description 4",
            Notes = "Test Notes 4",
            Completed = false,
            DueDate = DateTime.Now.AddDays(4),
            RepeatingInterval = TimeInterval.Weekly,
            Repititions = 5,
            Streak = 2,
        };

        void WriteFileForTests(string filename, int taskCount = 4)
        {
            var writer = new TaskDataFileWriter(filename);
            if (taskCount >= 1) writer.AddObjectToWrite(ExampleTask1);
            if (taskCount >= 2) writer.AddObjectToWrite(ExampleTask2);
            if (taskCount >= 3) writer.AddObjectToWrite(ExampleTask3);
            if (taskCount == 4) writer.AddObjectToWrite(ExampleTask4);
            if (taskCount > 4) throw new Exception("Not enough tasks");
            var filePath = writer.WriteValues();

            Assert.True(File.Exists(filePath));
            Debug.WriteLine($"File written to: {filePath}");
        }

        async Task WriteFileForTestsAsync(TaskDataFileWriter writer, int taskCount = 4)
        {
            if (taskCount >= 1) writer.AddObjectToWrite(ExampleTask1);
            if (taskCount >= 2) writer.AddObjectToWrite(ExampleTask2);
            if (taskCount >= 3) writer.AddObjectToWrite(ExampleTask3);
            if (taskCount == 4) writer.AddObjectToWrite(ExampleTask4);
            if (taskCount > 4) throw new Exception("Not enough tasks");
            var filePath = await writer.WriteValuesAsync();

            Assert.True(File.Exists(filePath));
            Debug.WriteLine($"File written to: {filePath}");
        }

        /// <summary>
        ///  This method needs to be replaced since it curently relies on the result from the other test
        /// </summary>
        [Fact]
        public void ReadDataFile_2Tasks_ReadSuccess()
        {
            var expecetdTasks = 2;
            var filename = $"testing-{expecetdTasks}_task-data_" + DateTime.Now.Ticks;
            WriteFileForTests(filename, expecetdTasks);

            var reader = new TaskDataFileReader(filename);
            var content = reader.ReadValues(); 

            Assert.Equal(expecetdTasks, content.Count);

            DebugPrintContent(content);
        }

        /// <summary>
        ///  This method needs to be replaced since it curently relies on the result from the other test
        /// </summary>
        [Fact]
        public void ReadDataFile_4Tasks_ReadSuccess()
        {
            var expecetdTasks = 4;
            var filename = $"testing-{expecetdTasks}_task-data_" + DateTime.Now.Ticks;

            WriteFileForTests(filename, expecetdTasks);

            var reader = new TaskDataFileReader(filename);
            var content = reader.ReadValues(); 

            Assert.Equal(expecetdTasks, content.Count);

            DebugPrintContent(content);
        }

        [Fact]
        public void ReadDataFile_RaceCondition_WillFail()
        {
            var expecetdTasks = 4;
            var filename = "testing-task-data_separate-threads_" + DateTime.Now.Ticks;
            Task.Run(() => WriteFileForTests(filename, expecetdTasks));

            var reader = new TaskDataFileReader(filename);
            Assert.ThrowsAsync<Exception>(async () =>
            {
                var count = 0;
                while (count < 1_000)
                {
                    try
                    {
                        var content = reader.ReadValues();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Exception caught: {ex.Message}");
                        if (ex.Message.Contains("is being used by another process"))
                        {
                            throw;
                        }
                        Debug.WriteLine("Exception thrown that file does not exist - ignoring");
                    }
                    count++;
                }
            });
        }

        /// <summary>
        /// Whilst the semaphore does cause access delay, we still can't handle the write delays of the storage drive this way
        /// </summary>
        [Fact]
        public void ReadDataFileAsync_TestingSemaphoreBehavior()
        {
            var filename = "testing-task-data_async_test_" + DateTime.Now.Ticks;

            var reader = new TaskDataFileReader(filename);
            var writer = new TaskDataFileWriter(filename);

            var expecetdTasks = 4;
            
            Task.Run(async () => {
                try
                {
                    await WriteFileForTestsAsync(writer, expecetdTasks);
                } catch (Exception ex)
                {
                    Debug.WriteLine($"Exception thrown during file write: {ex.Message}");
                }
            });

            var count = 0;
            //while (count != expecetdTasks)
            //{
                Task.Run(async () =>
                {
                    try
                    {
                        var content = await reader.ReadValuesAsync();
                        DebugPrintContent(content);
                        //count = content.Count;
                        Interlocked.Exchange(ref count, content.Count);
                    } catch (Exception ex)
                    {
                        Debug.WriteLine($"Exception thrown during file read: {ex.Message}");
                    }

                });
            //}
            Debug.WriteLine("Completed, results as expected");
        }

        static void DebugPrintContent(List<TaskDataEntity> content)
        {
            foreach (var item in content)
            {
                if (item is HabitualTaskDataEntity habitual)
                {
                    Debug.WriteLine($"ID={habitual.Id}, Desc={habitual.Description}, Notes={habitual.Notes}, Completed={habitual.Completed}, DueDate={habitual.DueDate.ToString()}, Interval={habitual.RepeatingInterval.ToString()}, Repititions={habitual.Repititions}, Streak={habitual.Streak}");
                }
                else if (item is RepeatingTaskDataEntity repeating)
                {
                    Debug.WriteLine($"ID={repeating.Id}, Desc={repeating.Description}, Notes={repeating.Notes}, Completed={repeating.Completed}, DueDate={repeating.DueDate.ToString()}, Interval={repeating.RepeatingInterval.ToString()}, Repititions={repeating.Repititions}");
                }
                else
                {
                    Debug.WriteLine($"ID={item.Id}, Desc={item.Description}, Notes={item.Notes}, Completed={item.Completed}, DueDate={item.DueDate.ToString()}");
                }
            }
        }
    }
}