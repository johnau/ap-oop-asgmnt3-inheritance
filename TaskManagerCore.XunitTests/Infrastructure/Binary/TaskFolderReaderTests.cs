using BinaryFileHandler;
using System.Diagnostics;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;

namespace TaskManagerCore.XunitTests.Infrastructure.Binary
{
    public class TaskFolderReaderTests
    {
        [Fact]
        public void ReadTaskFolders_2Folders_WillSucceed()
        {
            var filename = "reader-test_" + DateTime.Now.Ticks;
            var filePath = WriteTestFolders(filename);

            var conf = new BinaryFileConfig(filename);
            var reader = new TaskFolderFileReader(conf);
            var content = reader.ReadValues();

            Assert.Equal(2, content.Count);
            for (int i = 0; i < content.Count; i++)
            {
                var c = content[i];
                if (i == 0)
                {
                    Assert.Equal("test folder 1", c.Name);
                    Assert.Equal(3, c.TaskIds.Count);
                } else if (i == 1)
                {
                    Assert.Equal("test folder 2", c.Name);
                    Assert.Equal(4, c.TaskIds.Count);
                }
            }
        }

        string WriteTestFolders(string filename)
        {
            var folder1 = new TaskFolderEntity(Guid.NewGuid().ToString())
            {
                Name = "test folder 1",
                TaskIds = new List<string> { "taskId1", "taskId2", "taskId3" },
            };

            var folder2 = new TaskFolderEntity(Guid.NewGuid().ToString())
            {
                Name = "test folder 2",
                TaskIds = new List<string> { "taskId4", "taskId5", "taskId6", "taskId7" },
            };

            var conf = new BinaryFileConfig(filename);
            var writer = new TaskFolderFileWriter(conf);
            writer.AddObjectToWrite(folder1);
            writer.AddObjectToWrite(folder2);
            var filePath = writer.WriteValues();

            Assert.True(File.Exists(filePath));

            Debug.WriteLine($"File written to: {filePath}");

            Thread.Sleep(500);

            return filePath;
        }
    }
}
