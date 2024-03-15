using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;

namespace TaskManagerCore.XunitTests
{
    /// <summary>
    /// TODO....
    /// </summary>
    public class TaskDataDaoTests
    {
        [Fact]
        public void Save_WithValidEntity_WillReturnExpected()
        {
            var filename = "test-file-tasks";
            var testReader = new TaskDataFileReader(filename);
            var testWriter = new TaskDataFileWriter(filename);
            var dao = new TaskDataDao(testReader, testWriter);

            var entity = new TaskDataEntity() 
            {
                Description = "Test Task 1",
                Notes = "Test notes about task",
                Completed = false,
                DueDate = DateTime.Now,
            };

            var id = dao.Save(entity);
            Assert.NotEqual(string.Empty, id);
        }

        [Fact]
        public void FindByDueDate_WithValidDate_WillSucceed_500_times()
        {
            for (int i = 0; i < 500; i++)
            {
                FindByDueDate_WithValidDate_WillSucceed();
            }
        }


        [Fact]
        public void FindByDueDate_WithValidDate_WillSucceed()
        {
            var filename = "test-file-tasks_"+DateTime.Now.Ticks;
            var testReader = new TaskDataFileReader(filename);
            var testWriter = new TaskDataFileWriter(filename);
            var dao = new TaskDataDao(testReader, testWriter);

            var now = DateTime.Now;
            //PopulateTestTasks(dao, now);
            var daysAndQuantities = PopulateRandomTasksAndReturnQuantityOnEachDay(dao);

            foreach (var day in daysAndQuantities)
            {
                var dayIndex = day.Key;
                var expectedTaskQty = day.Value;
                Debug.WriteLine($"Expecting {expectedTaskQty} tasks on the {dayIndex} day");
                var dateMod = now.AddDays(dayIndex);
                var foundTasks = dao.FindByDueDate(dateMod);
                Debug.WriteLine($"Expecting {expectedTaskQty} {(expectedTaskQty != foundTasks.Count ? "But" : "And")} got {foundTasks.Count}");
                Assert.Equal(expectedTaskQty, foundTasks.Count);
            }

            //var tasksToday = dao.FindByDueDate(now);
            //Assert.Equal(3, tasksToday.Count);
            //var tasksInTwoDays = dao.FindByDueDate(now.AddDays(2));
            //Assert.Equal(2, tasksInTwoDays.Count);
            //var tasksInThreeDays = dao.FindByDueDate(now.AddDays(3));
            //Assert.Equal(3, tasksInThreeDays.Count);
        }

        Dictionary<int, int> PopulateRandomTasksAndReturnQuantityOnEachDay(TaskDataDao dao)
        {
            var dict = new Dictionary<int, int>();

            var today = DateTime.Now;
            var maxDays = 6;
            
            for (int i = 0; i <= maxDays; i++)
            {
                dict.Add(i, 0);
            }

            var random = new Random();
            for (int i = 1; i <= 10; i++)
            {
                int dayIndex = random.Next(0, maxDays+1);
                Debug.WriteLine($"Day Index: {dayIndex}");
                var date = today.AddDays(dayIndex);

                var entity = new TaskDataEntity()
                {
                    Description = $"Test Task ({Guid.NewGuid()})",
                    Notes = RandomString(),
                    Completed = RandomBool(),
                    DueDate = date,                 
                };
                dao.Save(entity);

                dict[dayIndex]++;
                Debug.WriteLine($"Now have {dict[dayIndex]} tasks on day {dayIndex}");
            }
            Debug.WriteLine($"Created {dict.Values.Sum()} tasks");
            foreach ( var t in dict )
            {
                Debug.WriteLine($"On day: {t.Key} there are {t.Value} tasks");
            }
            return dict;
        }

        static string RandomString()
        {
            Random random = new Random();
            string characters = "abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789";
            int length = random.Next(10, 30); // Desired length of the random string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(characters.Length);
                sb.Append(characters[index]);
            }

            return sb.ToString();
        }

        static bool RandomBool()
        {
            Random random = new Random();
            return random.Next(2) == 0;
        }

        void PopulateTestTasks(TaskDataDao dao, DateTime now)
        {
            var today = now;
            var twoDaysTime = now.AddDays(2);
            var threeDaysTime = now.AddDays(3);

            var entity = new TaskDataEntity()
            {
                Description = "Test Task 1",
                Notes = "Notes about test task",
                Completed = false,
                DueDate = twoDaysTime,                  //two
            };
            dao.Save(entity);

            var entity1 = new TaskDataEntity()
            {
                Description = "Test Task 2",
                Notes = "Notes about test task...............",
                Completed = true,
                DueDate = threeDaysTime,                //three
            };
            dao.Save(entity1);

            var entity2 = new TaskDataEntity()
            {
                Description = "Test Task 3",
                Notes = "Some other notes about test task",
                Completed = false,
                DueDate = today,                        //zero
            };
            dao.Save(entity2);

            var entity3 = new TaskDataEntity()
            {
                Description = "Test Task 4",
                Notes = "Test notes",
                Completed = true,
                DueDate = today,                        //zero
            };
            dao.Save(entity3);

            var entity3A = new TaskDataEntity()
            {
                Description = "Test Task 4A",
                Notes = "Test notes......",
                Completed = true,
                DueDate = today,                        //zero
            };
            dao.Save(entity3A);

            var entity4 = new TaskDataEntity()
            {
                Description = "Test Task 5",
                Notes = "Some other notes about test task",
                Completed = false,
                DueDate = threeDaysTime,                //three
            };
            dao.Save(entity4);

            var entity5 = new TaskDataEntity()
            {
                Description = "Test Task 6",
                Notes = "Notes about test task",
                Completed = true,
                DueDate = twoDaysTime,                  //two
            };
            dao.Save(entity5);

            var entity6 = new TaskDataEntity()
            {
                Description = "Test Task 7",
                Notes = "The 7th task added",
                Completed = true,
                DueDate = threeDaysTime,                //three
            };
            dao.Save(entity6);
        }
    }
}
