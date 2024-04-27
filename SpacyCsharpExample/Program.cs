using Catalyst;
using Mosaik.Core;
using Python.Runtime;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        Catalyst.Models.English.Register(); //You need to pre-register each language (and install the respective NuGet Packages)

        Storage.Current = new DiskStorage("catalyst-models");
        var nlp = await Pipeline.ForAsync(Language.English);
        var doc = new Document("The quick brown fox jumps over the lazy dog", Language.English);
        nlp.ProcessSingle(doc);
        Console.WriteLine(doc.ToJson());
    }
}