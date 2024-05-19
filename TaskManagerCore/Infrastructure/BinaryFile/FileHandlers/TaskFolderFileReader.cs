using BinaryFileHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers
{
    internal class TaskFolderFileReader : BinaryFileReader<TaskFolderEntity>
    {
        public TaskFolderFileReader(BinaryFileConfig config) : base(config) { }

        /// <summary>
        /// Read current object
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected override TaskFolderEntity ReadObject(BinaryReader reader)
        {
            var id = reader.ReadString();
            var name = reader.ReadString();
#if NETSTANDARD2_0
            
            var taskIds = new List<string>();
            var s = reader.ReadString();

            int delimiterIndex;
            int startIndex = 0;

            while ((delimiterIndex = s.IndexOf(Delimiter, startIndex)) != -1)
            {
                var taskId = s.Substring(startIndex, delimiterIndex - startIndex);
                if (!string.IsNullOrEmpty(taskId))
                {
                    taskIds.Add(taskId);
                }
                startIndex = delimiterIndex + Delimiter.Length;
            }

            var lastPart = s.Substring(startIndex);
            if (!string.IsNullOrEmpty(lastPart))
            {
                taskIds.Add(lastPart);
            }

#elif NET8_0_OR_GREATER
            var taskIds = reader.ReadString()
                .Split(Delimiter, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
#endif

            return new TaskFolderEntity(id)
            {
                Name = name,
                TaskIds = taskIds,
            };
        }
    }
}
