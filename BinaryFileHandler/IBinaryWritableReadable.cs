namespace BinaryFileHandler
{
    public interface IBinaryWritableReadable<T>
    {
        void WriteObject(BinaryWriter writer, T obj);
        void PopulateThis(BinaryReader reader);
    }
}
