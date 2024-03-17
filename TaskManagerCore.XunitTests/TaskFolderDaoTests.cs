using TaskManagerCore.Infrastructure.BinaryFile.Dao;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using TH = TaskManagerCore.XunitTests.TestHelpers.TestHelperFunctions;

namespace TaskManagerCore.XunitTests
{
    /// <summary>
    /// Test Task Folder Binary File DAO methods (Writes temp files)
    /// 
    /// Tests the Sort and Search requirements for Assessment 5
    /// </summary>
    public class TaskFolderDaoTests
    {
        private const int _testIterations = 20;

        [Fact]
        public void Save_WithValidEntity_WillReturnExpected()
        {
            (var filename, var dao) = SetupTaskFolderDao("test-folder");

            var entity = new TaskFolderEntity()
            {
                Name = "Folder 1",
                TaskIds = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() },
            };

            var id = dao.Save(entity);
            Assert.NotEqual(string.Empty, id);
            Assert.Equal(entity.Id, id);

            TH.CleanupAfterTest(filename);
        }

        [Fact]
        public void FindByName_WithValidName_WillSuceed()
        {
            TH.RunTestMultipleTimes(() =>
            {
                (var filename, var dao) = SetupTaskFolderDao("test-folder-1");
                var folderNamesAndIdCounts = PopulateTestFolders(dao, 25);
                (var nameFragment, var count) = TH.MostCommonStartingWords(folderNamesAndIdCounts.Keys.ToList());

                var result = dao.FindByName(nameFragment);
                Assert.Equal(count, result.Count);

                TH.CleanupAfterTest(filename);

            }, _testIterations);
        }

        [Fact]
        public void FindOneByName_WithValidName_WillSuceed()
        {
            TH.RunTestMultipleTimes(() =>
            {
                (var filename, var dao) = SetupTaskFolderDao("test-folder-2");
                var folderNamesAndIdCounts = PopulateTestFolders(dao, 25);
                var random = new Random();
                var randomIndex = random.Next(folderNamesAndIdCounts.Count);
                var randomElement = folderNamesAndIdCounts.ElementAt(randomIndex);

                var result = dao.FindOneByName(randomElement.Key);
                Assert.NotNull(result);
                Assert.Equal(randomElement.Key, result.Name);

                TH.CleanupAfterTest(filename);

            }, _testIterations);
        }

        [Fact]
        public void FindEmpty_WillSuceed()
        {
            TH.RunTestMultipleTimes(() =>
            {
                (var filename, var dao) = SetupTaskFolderDao("test-folder-3");
                var folderNamesAndTaskCounts = PopulateTestFolders(dao, 25);
                var emptyCount = folderNamesAndTaskCounts
                    .Where(entry => entry.Value.Count == 0)
                    .Count();

                var results = dao.FindEmpty();

                Assert.Equal(emptyCount, results.Count);

                TH.CleanupAfterTest(filename);
            
            }, _testIterations);
        }

        [Fact]
        public void FindNotEmpty_WillSuceed()
        {
            TH.RunTestMultipleTimes(() =>
            {
                (var filename, var dao) = SetupTaskFolderDao("test-folder-4");
                var folderNamesAndTaskCounts = PopulateTestFolders(dao, 25);
                var emptyCount = folderNamesAndTaskCounts
                    .Where(entry => entry.Value.Count > 0)
                    .Count();

                var results = dao.FindNotEmpty();

                Assert.Equal(emptyCount, results.Count);

                TH.CleanupAfterTest(filename);

            }, _testIterations);
        }

        [Fact]
        public void FindParentOfTask_WithValidTaskId_WillSuceed()
        {
            (var filename, var dao) = SetupTaskFolderDao("test-folder-5");
            var namesAndIds = PopulateTestFolders(dao, 10);

            var random = new Random();
            var element = namesAndIds.ElementAt(random.Next(namesAndIds.Count));
            while (element.Value.Count == 0)
            {
                element = namesAndIds.ElementAt(random.Next(namesAndIds.Count));
            }
            var folderName = element.Key;
            var folderTasks = element.Value;
            var rndTaskId = folderTasks[random.Next(folderTasks.Count)];

            var found = dao.FindParentOfTask(rndTaskId);
            Assert.NotNull(found);
            Assert.Equal(folderName, found.Name);

            TH.CleanupAfterTest(filename);
        }

        #region static test helpers
        static (string filename, TaskFolderDao dao) SetupTaskFolderDao(string testName)
        {
            var filename = testName + "_" + DateTime.Now.Ticks;
            var testReader = new TaskFolderFileReader(filename);
            var testWriter = new TaskFolderFileWriter(filename);
            var dao = new TaskFolderDao(testReader, testWriter);
            return (filename, dao);
        }

        static Dictionary<string, List<string>> PopulateTestFolders(TaskFolderDao dao, int count = 20)
        {
            var namesAndIds = new Dictionary<string, List<string>>();
            var random = new Random();
            for (int i = 0; i < count; i++)
            {
                var idList = new List<string>();
                for (int j = 0; j < random.Next(10); j++)
                {
                    if (random.Next(10) == 1) break; // Increase chances of empty
                    idList.Add(Guid.NewGuid().ToString());
                }
                var folderName = TH.RandomStringOfWords();
                var folder = new TaskFolderEntity()
                {
                    Name = folderName,
                    TaskIds = idList
                };

                var id = dao.Save(folder);
                namesAndIds.Add(folderName, idList);
            }
            return namesAndIds;
        }

        #endregion
    }
}
