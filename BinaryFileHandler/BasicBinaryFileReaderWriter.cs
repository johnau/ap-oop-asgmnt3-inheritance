namespace BinaryFileHandler
{
    /// <summary>
    /// Concrete Binary File Reader and Writer that takes IBinaryWritable<typeparamref name="T"/> classes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasicBinaryFileReaderWriter<T> : BinaryFileAccessor, IBinaryFileReader<T>, IBinaryFileWriter<T> 
        where T : IBinaryWritableReadable<T>
    {
        public BasicBinaryFileReaderWriter(BinaryFileConfig config) : base(config)
        {
        }

        public List<T> Data => throw new NotImplementedException();

        public void AddObjectsToWrite(List<T> items)
        {
            throw new NotImplementedException();
        }

        public void AddObjectToWrite(T item)
        {
            throw new NotImplementedException();
        }

        public List<T> ReadValues()
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> ReadValuesAsync()
        {
            throw new NotImplementedException();
        }

        public bool TryWriteFile(List<T> toWrite, int retriesRemaining = 20)
        {
            throw new NotImplementedException();
        }

        public bool WriteValues()
        {
            throw new NotImplementedException();
        }

        public Task WriteValuesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
