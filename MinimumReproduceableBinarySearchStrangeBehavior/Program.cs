// See https://aka.ms/new-console-template for more information
using MinimumReproduceableBinarySearchStrangeBehavior;
using System.Globalization;

Console.WriteLine("Hello, World!");

Console.WriteLine("The conclusion is BinarySearch matches a random index in the range of matches in a sorted list");
Console.WriteLine("Must search left and right from the matched index to find the other matches");

//// Example Description and notes to use for testing
//var descriptions = new List<string>() {
//    "Test 1",
//    "Test 2",
//    "Test 3",
//    "This is a test",
//    "Some other description",
//    "This is an Example Description",
//    "Example Description",
//    "A description"
//};
//var notes = new List<string>() { 
//    "Some notes about the data",
//    "Some other notes about the data",
//    "Also some more notes",
//    "Data notes",
//    "A note for testing",
//    "A note about testing",
//};

// Main list to be used for other sorted lists
var mainList = new List<DataObject>
{
    // Populate the list
    new DataObject("Description A",     "Some notes about the data"),
    new DataObject("Some description",  "Some other notes about the data"),
    new DataObject("A Test",            "Also more notes"),
    new DataObject("Description 2",     "A note for testing"),
    new DataObject("Some other description", "A note about testing"),
    new DataObject("Description 3",     "Data notes"),
    new DataObject("A test description", "Z last note"),
    new DataObject("Z last description", "Some other notes"),
};

foreach (var item in mainList)
{
    Console.WriteLine(item);
}
Console.WriteLine("");

//var random = new Random();
//for (int i = 0; i < 20; i++)
//{
//    var iDescription = random.Next(0, descriptions.Count+1);
//    var iNote = random.Next(0, notes.Count+1);
//    var dataObject = new DataObject(descriptions[iDescription], notes[iNote]);
//    mainList.Add(dataObject);

//    Console.WriteLine($"{i}: {dataObject}");
//}

// Sort the lists
//var sortedLists = new Dictionary<string, List<DataObject>>();

// Sort by description
var sortedByDescription = new List<DataObject>(mainList);
sortedByDescription.Sort(DataObject.CompareByDescription);

for (int i = 0; i < sortedByDescription.Count; i++)
{
    Console.WriteLine($"{i}:\t{sortedByDescription[i]}");
}
Console.WriteLine("");

//sortedLists.Add("description", sortedByDescription);

// Sort by notes
var sortedByNotes = new List<DataObject>(mainList);
sortedByNotes.Sort(DataObject.CompareByNotes);

for (int i = 0; i < sortedByNotes.Count; i++)
{
    Console.WriteLine($"{i}:\t{sortedByNotes[i]}");
}
Console.WriteLine("");

//List<DataObject> d_results1 = DoDescriptionSearch("a test", sortedByDescription);
//HandleResult(d_results1, 2);
//List<DataObject> d_results2 = DoDescriptionSearch("description", sortedByDescription);
//HandleResult(d_results2, 3);
//List<DataObject> d_results3 = DoDescriptionSearch("some", sortedByDescription);
//HandleResult(d_results3, 2);

//List<DataObject> n_results1 = DoNotesSearch("some", sortedByNotes);
//HandleResult(n_results1, 3);
//List<DataObject> n_results2 = DoNotesSearch("a note", sortedByNotes);
//HandleResult(n_results2, 2);

var r1 = sortedByDescription.DoBinarySearchFindAll(new DataObject("a test", ""), new DescriptionStartsWithComparer());
HandleResult(r1, 2);
var r2 = sortedByDescription.DoBinarySearchFindAll(new DataObject("description", ""), new DescriptionStartsWithComparer());
HandleResult(r2, 3);
var r3 = sortedByDescription.DoBinarySearchFindAll(new DataObject("some", ""), new DescriptionStartsWithComparer());
HandleResult(r3, 2);

var r4 = sortedByNotes.DoBinarySearchFindAll(new DataObject("", "some"), new NotesStartsWithComparer());
HandleResult(r4, 3);
var r5 = sortedByNotes.DoBinarySearchFindAll(new DataObject("", "a note"), new NotesStartsWithComparer());
HandleResult(r5, 2);

static void HandleResult(List<DataObject> results, int expectedCount)
{
    Console.WriteLine($"Expected: {expectedCount}, got {results.Count}");
    for (int i = 0; i < results.Count; i++)
    {
        Console.WriteLine($"{results[i]}");
    }
}

static List<DataObject> DoDescriptionSearch(string search, List<DataObject> sorted)
{
    var sortedCopy = new List<DataObject>(sorted);
    sortedCopy.Insert(0, new DataObject("", ""));

    var criteria = new DataObject(search, "");
    var matches = new List<DataObject>();

    var firstMatch = sortedCopy.BinarySearch(criteria, new DescriptionStartsWithComparer());
    matches.Add(sortedCopy[firstMatch]);

    if (firstMatch < 0) return matches;

    int current = firstMatch;
    while (current+1 < sortedCopy.Count)
    {
        current = sortedCopy.BinarySearch(++current, 1, criteria, new DescriptionStartsWithComparer());
        if (current < 0) break;
        
        matches.Add(sortedCopy[current]);
    }
    Console.WriteLine($"\nMatched {matches.Count} items with Description search: {search}\n");
    return matches;
}

static List<DataObject> DoNotesSearch(string search, List<DataObject> sorted)
{
    var sortedCopy = new List<DataObject>(sorted);
    //sortedCopy.Insert(0, new DataObject("", ""));

    var criteria = new DataObject("", search);
    var matches = new List<DataObject>();

    var firstMatch = sortedCopy.BinarySearch(criteria, new DescriptionStartsWithComparer());
    matches.Add(sortedCopy[firstMatch]);

    if (firstMatch < 0) return matches;

    int current = firstMatch;
    while (current + 1 < sortedCopy.Count)
    {
        current = sortedCopy.BinarySearch(++current, 1, criteria, new NotesStartsWithComparer());
        if (current < 0) break;

        matches.Add(sortedCopy[current]);
    }
    Console.WriteLine($"\nMatched {matches.Count} items with Notes search: {search}\n");
    return matches;
}