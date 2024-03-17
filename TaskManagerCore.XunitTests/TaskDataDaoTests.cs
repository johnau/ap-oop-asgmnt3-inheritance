using BinaryFileHandler;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile;
using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TH = TaskManagerCore.XunitTests.TestHelpers.TestHelperFunctions;

namespace TaskManagerCore.XunitTests
{
    /// <summary>
    /// Test Task Binary File DAO methods (Writes temp files)
    /// 
    /// Tests the Sort and Search requirements for Assessment 5
    /// </summary>
    public class TaskDataDaoTests
    {
        private const int _testIterations = 5;

        [Fact]
        public void Save_WithValidEntity_WillReturnExpected()
        {
            (var filename, var dao) = SetupTaskDataDao("test-tasks-notes");

            // Create dummy entity, save, and verify
            var entity = new TaskDataEntity() 
            {
                Description = "Test Task 1",
                Notes = "Test notes about task",
                Completed = false,
                DueDate = DateTime.Now,
            };

            var id = dao.Save(entity);

            Assert.NotEqual(string.Empty, id);
            Assert.Equal(entity.Id, id);

            TH.CleanupAfterTest(filename);
        }

        [Fact]
        public void FindByDueDate_RandomizedTest_WillMatchMultiple()
        {
            TH.RunTestMultipleTimes(() =>
            {
                (var filename, var dao) = SetupTaskDataDao("test-tasks-notes1");
                var daysAndQuantities = PopulateRandomTestTasks(dao, 20).DaysToTaskCounts;

                foreach (var day in daysAndQuantities)
                {
                    var dayIndex = day.Key;
                    var expectedTaskQty = day.Value;

                    Debug.WriteLine($"Expecting {expectedTaskQty} tasks on the {dayIndex} day");
                    
                    var dateMod = DateTime.Now.AddDays(dayIndex);
                    var foundTasks = dao.FindByDueDate(dateMod);
                    
                    Debug.WriteLine($"Expecting {expectedTaskQty} {(expectedTaskQty != foundTasks.Count ? "But" : "And")} got {foundTasks.Count}");
                    
                    Assert.Equal(expectedTaskQty, foundTasks.Count);
                }

                TH.CleanupAfterTest(filename);

            }, _testIterations);
        }

        [Theory]
        [InlineData("This is a test", 4)]
        [InlineData("Test Task", 3)]
        [InlineData("zzzz", 1)]
        [InlineData("this is a test task", 2)]
        public void FindByDescription_WithDescriptionFragment_WillMatchMultiple(string description, int count)
        {
            (var filename, var dao) = SetupTaskDataDao("test-tasks-notes2");

            PopulateTestTasks(dao);

            var results = dao.FindByDescription(description);
            Assert.Equal(count, results.Count);

            TH.CleanupAfterTest(filename);
        }

        [Theory]
        [InlineData("notes about test", 3)]
        [InlineData("Some other notes", 2)]
        [InlineData("The 7th task", 1)]
        [InlineData("A task", 1)]
        public void FindByNotes_WithNotesFragment_WillMatchMultiple(string notes, int count)
        {
            (var filename, var dao) = SetupTaskDataDao("test-tasks-notes3");
            PopulateTestTasks(dao);

            var results = dao.FindByNotes(notes);
            Assert.Equal(count, results.Count);

            TH.CleanupAfterTest(filename);
        }

        [Fact]
        public void FindByDescription_WithRandomizedValues_WillMatchMultiple()
        {
            TH.RunTestMultipleTimes(() =>
            {
                (var filename, var dao) = SetupTaskDataDao("test-tasks-notes4");
                var idsToDescriptions = PopulateRandomTestTasks(dao, 20).IdsToDescriptions;

                (string descriptionFragment, int count) = TH.MostCommonStartingWords(idsToDescriptions.Values.ToList(), 1);

                var results = dao.FindByDescription(descriptionFragment);
                Assert.Equal(count, results.Count);

                TH.CleanupAfterTest(filename);

            }, _testIterations);
        }

        [Fact]
        public void FindByNotes_WithRandomizedValues_WillMatchMultiple()
        {
            TH.RunTestMultipleTimes(() =>
            {
                (var filename, var dao) = SetupTaskDataDao("test-tasks-notes4");
                var idsToNotes = PopulateRandomTestTasks(dao, 20).IdsToNotes;

                (string noteFragment, int count) = TH.MostCommonStartingWords(idsToNotes.Values.ToList(), 2);

                var results = dao.FindByNotes(noteFragment);
                Assert.Equal(count, results.Count);

                TH.CleanupAfterTest(filename);
            
            }, _testIterations);
        }

        [Fact]
        public void FindByCompleted_RandomizedTest_WillSucceed()
        {
            static void TestMethod(bool b)
            {
                (var filename, var dao) = SetupTaskDataDao("test-tasks-notes5");
                var completedTaskCounts = PopulateRandomTestTasks(dao, 20).CompletedToTaskCounts;

                var results = dao.FindByCompleted(b);
                Assert.Equal(completedTaskCounts[b], results.Count);

                TH.CleanupAfterTest(filename);
            }

            TH.RunTestMultipleTimes(() => TestMethod(true), _testIterations);
            TH.RunTestMultipleTimes(() => TestMethod(false), _testIterations);
        }

        #region static test helpers
        /// <summary>
        /// Build testing class
        /// </summary>
        /// <param name="testName"></param>
        /// <returns></returns>
        static (string filename, TaskDataDao dao) SetupTaskDataDao(string testName)
        {
            var filename = testName + "_" + DateTime.Now.Ticks;
            var conf = new BinaryFileConfig(filename);
            var testReader = new TaskDataFileReader(conf);
            var testWriter = new TaskDataFileWriter(conf);
            var dao = new TaskDataDao(testReader, testWriter);
            return (filename, dao);
        }

        struct PopulatedResults
        {
            public Dictionary<int, int> DaysToTaskCounts { get; set; }
            public Dictionary<bool, int> CompletedToTaskCounts { get; set; }
            public Dictionary<string, string> IdsToNotes { get; set; }
            public Dictionary<string, string> IdsToDescriptions { get; set; }
        }

        /// <summary>
        /// Create some randomized objects in the database
        /// </summary>
        /// <param name="dao"></param>
        /// <param name="sizeFactor"></param>
        /// <returns></returns>
        static PopulatedResults PopulateRandomTestTasks(TaskDataDao dao, int sizeFactor = 10)
        {
            // Result collections
            var daysTaskCounts = new Dictionary<int, int>();
            var completedTaskCounts = new Dictionary<bool, int>();
            var idsToNotes = new Dictionary<string, string>();
            var idsToDescriptions = new Dictionary<string, string>();

            var today = DateTime.Now;
            var dayWithNoTasks = 2;
            var maxDays = 10; // Max days must not be less than dayWithNoTasks

            // Initialize results
            for (int i = 0; i < maxDays; i++) 
                daysTaskCounts.Add(i, 0); // Prefill daysTaskCounts with 0's

            completedTaskCounts.Add(true, 0);
            completedTaskCounts.Add(false, 0);
            
            // Create some random Task Entities
            var random = new Random();
            for (int i = 1; i <= sizeFactor; i++)
            {
                // For random Due Date
                int dayIndex = random.Next(0, maxDays);
                if (dayIndex == dayWithNoTasks) dayIndex = 0; // making sure there is a missing day

                // Randomized values for Task properties
                var description = TH.RandomStringOfWords(2, 5) + $"_{Guid.NewGuid()}";
                var date = today.AddDays(dayIndex);
                var completed = TH.RandomBool();
                var notes = TH.RandomStringOfWords(5, 20);
                // Create test entity
                var entity = new TaskDataEntity()
                {
                    Description = description,
                    Notes = notes,
                    Completed = completed,
                    DueDate = date,                 
                };
                // Store with DAO
                var id = dao.Save(entity);

                // Store locally for tests
                daysTaskCounts[dayIndex]++;
                completedTaskCounts[completed]++;
                idsToNotes.Add(id, notes);
                idsToDescriptions.Add(id, description);

                Debug.WriteLine($"Now have {daysTaskCounts[dayIndex]} tasks on day {dayIndex}");
            }

            // Debug out
            Debug.WriteLine($"Created {daysTaskCounts.Values.Sum()} tasks");
            foreach ( var t in daysTaskCounts )
                Debug.WriteLine($"On day: {t.Key} there are {t.Value} tasks");

            // Populate result object
            return new PopulatedResults
            {
                DaysToTaskCounts = daysTaskCounts,
                CompletedToTaskCounts = completedTaskCounts,
                IdsToNotes = idsToNotes,
                IdsToDescriptions = idsToDescriptions
            };
        }

        /// <summary>
        /// This method has now been replaced by the PopulateRandomTestTasks method
        /// </summary>
        /// <param name="dao"></param>
        static void PopulateTestTasks(TaskDataDao dao)
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
        #endregion
    }
}
