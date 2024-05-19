
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BinaryFileHandler
{
    public interface IBinaryFileReader<T>
    {
        List<T> Data { get; }
        List<T> ReadValues();
        Task<List<T>> ReadValuesAsync();
    }
}