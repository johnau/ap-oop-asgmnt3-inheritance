namespace BinaryFileHandler
{
    public interface IBinaryWritableReadable<T>
    {
        void WriteObject(BinaryWriter writer, T obj);
        T WithDataFromBinaryReader(BinaryReader reader, string className);
    }
}
