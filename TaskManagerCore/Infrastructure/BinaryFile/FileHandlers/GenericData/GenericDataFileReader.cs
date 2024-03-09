using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagerCore.Infrastructure.BinaryFile.FileHandlers.Helper;

namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers.GenericData
{
    internal class GenericDataFileReader : GenericBinaryFileAccessor
    {
        protected Dictionary<string, Entry> CurrentData;
        public Dictionary<string, Entry> Data => new Dictionary<string, Entry>(CurrentData);
        protected GenericDataFileReader(Dictionary<string, Entry> dataStructure, string filename = "data", string? rootPath = null)
            : base(dataStructure, filename, rootPath)
        {
            CurrentData = new Dictionary<string, Entry>();
        }

        public Dictionary<string, Entry> ReadValues()
        {
            var filePath = GenerateFilePath();
            if (!FileExists(filePath))
                throw new Exception($"An expected file was not found: {filePath}");

            // Retry a few times incase the file is still being written.  100 tries at 100ms = 10000ms = 10s, already too long...
            var retryAttempts = 100;
            while (retryAttempts > 0)
            {
                try
                {
                    //using (var stream = File.Open(filePath, FileMode.Open))
                    //using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize: 4096, useAsync: true))
                    using (var stream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true))
                    using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
                    {
                        while (HasNext(reader))
                        {
                            (string name, Entry entry) = ReadData(reader);
                            CurrentData.TryAdd(name, entry);
                        }
                    }
                    retryAttempts = 0;
                    return new Dictionary<string, Entry>(CurrentData);
                }
                catch (IOException ex) when (IsFileInUse(ex)) // if file is in use we will retry
                {
                    retryAttempts--;
                    Debug.WriteLine("File is in use, waiting 100ms and retrying...");
                    Thread.Sleep(100);
                }
                catch (Exception ex) // catch all other exceptions and fail
                {
                    Debug.WriteLine($"An exception was thrown: {ex.Message} whil trying to read the file {filePath}");
                    throw;
                }
            }

            throw new Exception($"Unable to read from file: {filePath}");
        }

        /// <summary>
        /// Need to re-write how the data structure is handled, builder pattern in the base class
        /// Construct the data structure and then read/write in that order.
        /// Is this level of generic-ness beneficial?  I don't think so...
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private bool HasNext(BinaryReader reader)
        {
            foreach (var entryDetails in DataStructure)
            {
                var name = entryDetails.Key;
                var type = entryDetails.Value.Type;
                object? obj = null;
                switch (type.FullName)
                {
                    case "System.String":
                        obj = reader.ReadString();
                        break;
                    case "System.Int16":
                        obj = reader.ReadInt16();
                        break;
                    case "System.Int32":
                        obj = reader.ReadInt32();
                        break;
                    case "System.Int64":
                        obj = reader.ReadInt64();
                        break;
                    case "System.Boolean":
                        obj = reader.ReadBoolean();
                        break;
                    case "System.Decimal":
                        obj = reader.ReadDecimal();
                        break;
                    case "System.Double":
                        obj = reader.ReadDouble();
                        break;
                    case "System.Half":
                        obj = reader.ReadHalf();
                        break;
                    case "System.Char":
                        obj = reader.ReadChar();
                        break;
                    case "System.Byte":
                        obj = reader.ReadByte();
                        break;
                    default:
                        throw new Exception("Not all types are handled");
                }

                if (name.ToLower() == "id"
                    && type.FullName.ToLower().Equals("system.string")
                    && (string)obj == GenericTerminators.StringTerminator)
                {
                    return false;
                }
                CurrentData.TryAdd(name, new Entry(type, obj));
            }

            throw new NotImplementedException();
        }

        private (string name, Entry entry) ReadData(BinaryReader reader)
        {
            throw new NotImplementedException();
            //return ("", new Entry());
        }
    }
}
