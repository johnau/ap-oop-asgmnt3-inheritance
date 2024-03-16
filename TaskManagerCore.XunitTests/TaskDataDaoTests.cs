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

            File.Delete(filename);
        }

        [Fact]
        public void FindByDueDate_WithValidDate_WillMatchMultiple()
        {
            var test = () =>
            {
                var filename = "test-file-tasks_" + DateTime.Now.Ticks;
                var testReader = new TaskDataFileReader(filename);
                var testWriter = new TaskDataFileWriter(filename);
                var dao = new TaskDataDao(testReader, testWriter);

                var now = DateTime.Now;
                var daysAndQuantities = PopulateRandomTasksAndReturnQuantityOnEachDay(dao, 20);

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

                File.Delete(filename);
            };

            for (int i = 0; i < 20; i++)
            {
                Debug.WriteLine($"Test iteration: {i}");
                test();
            }
        }

        [Theory]
        [InlineData("This is a test", 4)]
        [InlineData("Test Task", 3)]
        [InlineData("zzzz", 1)]
        [InlineData("this is a test task", 2)]
        public void FindByDescription_WithDescriptionFragment_WillMatchMultiple(string description, int count)
        {
            var filename = "test-file-tasks_2_" + DateTime.Now.Ticks;
            var testReader = new TaskDataFileReader(filename);
            var testWriter = new TaskDataFileWriter(filename);
            var dao = new TaskDataDao(testReader, testWriter);

            PopulateTestTasks(dao);

            var results = dao.FindByDescription(description);
            Assert.Equal(count, results.Count);

            File.Delete(filename);
        }

        [Theory]
        [InlineData("notes about test", 3)]
        [InlineData("Some other notes", 2)]
        [InlineData("The 7th task", 1)]
        [InlineData("A task", 1)]
        public void FindByNotes_WithNotesFragment_WillMatchMultiple(string notes, int count)
        {
            var filename = "test-file-tasks_3_" + DateTime.Now.Ticks;
            var testReader = new TaskDataFileReader(filename);
            var testWriter = new TaskDataFileWriter(filename);
            var dao = new TaskDataDao(testReader, testWriter);

            PopulateTestTasks(dao);

            var results = dao.FindByNotes(notes);
            Assert.Equal(count, results.Count);

            File.Delete(filename);
        }

        Dictionary<int, int> PopulateRandomTasksAndReturnQuantityOnEachDay(TaskDataDao dao, int sizeFactor = 10)
        {
            var dict = new Dictionary<int, int>();

            var today = DateTime.Now;
            var maxDays = 6;
            
            for (int i = 0; i <= maxDays; i++)
            {
                dict.Add(i, 0);
            }

            var random = new Random();
            for (int i = 1; i <= sizeFactor; i++)
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

        /// <summary>
        /// Changing this method may break some tests, as some theories rely on 
        /// this test data containing certain numbers of certain values.
        /// </summary>
        /// <param name="dao"></param>
        void PopulateTestTasks(TaskDataDao dao)
        {
            var today = DateTime.Now;
            var twoDaysTime = today.AddDays(2);
            var threeDaysTime = today.AddDays(3);

            var entity = new TaskDataEntity()
            {
                Description = "This is a test task description",
                Notes = "Notes about test task",
                Completed = false,
                DueDate = twoDaysTime,                  //two
            };
            dao.Save(entity);

            var entity1 = new TaskDataEntity()
            {
                Description = "Test Task 2",
                Notes = "Notes about test task ...............",
                Completed = true,
                DueDate = threeDaysTime,                //three
            };
            dao.Save(entity1);

            var entity2 = new TaskDataEntity()
            {
                Description = "Test Task 3",
                Notes = "Some other notes for test task",
                Completed = false,
                DueDate = today,                        //zero
            };
            dao.Save(entity2);

            var entity3 = new TaskDataEntity()
            {
                Description = "This is a test task",
                Notes = "Test notes",
                Completed = true,
                DueDate = today,                        //zero
            };
            dao.Save(entity3);

            var entity4 = new TaskDataEntity()
            {
                Description = "This is a test...",
                Notes = "Some other notes for test task",
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
                Description = "This is a TEST!!!!!!",
                Notes = "The 7th task added",
                Completed = true,
                DueDate = threeDaysTime,                //three
            };
            dao.Save(entity6);

            var entity7 = new TaskDataEntity()
            {
                Description = "ZZZZ description",
                Notes = "A task note",
                Completed = true,
                DueDate = threeDaysTime,                //three
            };
            dao.Save(entity7);
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
    }
}
