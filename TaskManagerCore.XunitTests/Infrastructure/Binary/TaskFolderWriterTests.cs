using BinaryFileHandler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerCore.Infrastructure.BinaryFile;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers;
using Xunit;

namespace TaskManagerCore.XunitTests.Infrastructure.Binary
{
    public class TaskFolderWriterTests
    {
        [Fact]
        public void WriteTaskFolders_WillSucceed()
        {
            var folder1 = new TaskFolderEntity(Guid.NewGuid().ToString())
            {
                Name = "test folder 1",
                TaskIds = new List<string> { "taskId1", "taskId2", "taskId3" },
            };

            var folder2 = new TaskFolderEntity(Guid.NewGuid().ToString())
            {
                Name = "test folder 2",
                TaskIds = new List<string> { "taskId4", "taskId5", "taskId6" },
            };

            var conf = new BinaryFileConfig("testing-task-folders");
            var writer = new TaskFolderFileWriter(conf);
            writer.AddObjectToWrite(folder1);
            writer.AddObjectToWrite(folder2);
            var success = writer.WriteValues();

            Assert.True(File.Exists(writer.FilePath));

            Debug.WriteLine($"File written to: {writer.FilePath}");
        }
    }
}
