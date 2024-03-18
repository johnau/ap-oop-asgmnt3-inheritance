namespace BinaryFileHandler
{
    public interface IBinaryWritableReadable<T>
    {
        void WriteObject(BinaryWriter writer, T obj);
        T PopulateThis(BinaryReader reader, string className);
    }
}
