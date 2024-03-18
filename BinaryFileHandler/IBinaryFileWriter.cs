
namespace BinaryFileHandler
{
    public interface IBinaryFileWriter<T>
    {
        void AddObjectsToWrite(List<T> items);
        void AddObjectToWrite(T item);
        bool TryWriteFile(List<T> toWrite, int retriesRemaining = 20);
        bool WriteValues();
        Task WriteValuesAsync();
    }
}