using System.IO;

namespace BinaryFileHandler
{
    public interface IBinaryWritableReadable<T>
    {
        void WriteObject(BinaryWriter writer, T obj);
        T ReadObject(BinaryReader reader, string className);
    }
}
