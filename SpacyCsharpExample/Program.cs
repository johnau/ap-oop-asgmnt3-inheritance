using Catalyst;
using Mosaik.Core;
using Python.Runtime;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("spaCY in .net");
        Catalyst.Models.English.Register(); //You need to pre-register each language (and install the respective NuGet Packages)

        Storage.Current = new DiskStorage("catalyst-models");

        await ProcessString("Call Rob at 3pm next tuesday");
        await ProcessString("Create a task about doing the very important project on Tuesday at 3pm");
    }

    private static async Task ProcessString(string input)
    {
        var nlp = await Pipeline.ForAsync(Language.English);
        var doc = new Document(input, Language.English);
        nlp.ProcessSingle(doc);
        //Console.WriteLine(doc.ToJson());

        foreach (var tokenData in doc.TokensData)
        {
            foreach (var t in tokenData)
            {
                var tokenStr = input.Substring(t.LowerBound, t.UpperBound - t.LowerBound + 1);
                Console.WriteLine($"Token: {tokenStr, -10} Tag: {t.Tag, -5} [{t.LowerBound}:{t.UpperBound}]");
            }
        }

        Console.WriteLine("");

        //foreach (var entity in doc.EntityData)
        //{
        //    Console.WriteLine($"Entity {entity.Key}");
        //    foreach (var type in entity.Value)
        //    {
        //        Console.WriteLine($"Metadata: {type.Metadata}, Tag: {type.Tag} Target: {type.TargetUID}");
        //    }
        //}

        //foreach (var entity in doc.TokenMetadata)
        //{
        //    Console.WriteLine($"Entity {entity.Key}");
        //    foreach (var kvp in entity.Value)
        //    {
        //        Console.WriteLine($"Metadata: {kvp.Key} : {kvp.Value}");
        //    }
        //}

        //Console.WriteLine("");
        //foreach (Token token in doc.ToTokenList())
        //{
        //    //Console.WriteLine($"Token Head: {token.Head}");
        //    //var lexeme = doc.
        //    //Console.WriteLine($@"{word.Text} {lexeme.Orth} {lexeme.Shape} {lexeme.Prefix} {lexeme.Suffix} {lexeme.IsAlpha} {lexeme.IsDigit} {lexeme.IsTitle} {lexeme.Lang}");
        //    Console.WriteLine($"'{token.Value}' ({token.Lemma}:{token.Index})\t{token.POS}\t'{token.Replacement}'\tType: {token.DependencyType}");
        //    foreach (var kvp in token.Metadata)
        //    {
        //        Console.WriteLine($"{kvp.Key} : {kvp.Value}");
        //    }
        //}
    }
}