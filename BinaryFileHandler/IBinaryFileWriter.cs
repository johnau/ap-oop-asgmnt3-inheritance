
namespace BinaryFileHandler
{
    public interface IBinaryFileWriter<T>
    {
        void AddObjectsToWrite(List<T> items);
        void AddObjectToWrite(T item);
        bool WriteValues();
        Task WriteValuesAsync();
    }
}