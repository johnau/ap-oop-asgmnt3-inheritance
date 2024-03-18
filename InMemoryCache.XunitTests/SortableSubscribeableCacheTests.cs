using System.Diagnostics;

namespace InMemoryCache.XunitTests
{
    public class SortableSubscribeableCacheTests
    {
        public class TestObject : IComparable<TestObject>, IIdentifiable
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public DateTime? Date { get; set; }
            public bool Completed { get; set; }

            public TestObject(string id, string description, DateTime? date, bool completed)
            {
                Id = id;
                Description = description;
                Date = date;
                Completed = completed;
            }

            public int CompareTo(TestObject? other)
            {
                if (other == null) return 1;

                var compareCompleted = Completed.CompareTo(other.Completed);
                if (compareCompleted != 0) return compareCompleted;

                var compareDescription = string.Compare(Description, other.Description, StringComparison.OrdinalIgnoreCase);
                if (compareDescription != 0) return compareDescription;

                // This is ugly but is to protect against DueDate being populated with a DateTime(0) object instead of null
                // ... Have now added protection to the setter, but will leave this here for now.
                if (Date.HasValue && Date.Value > DateTime.MinValue
                    && other.Date.HasValue && other.Date.Value > DateTime.MinValue)
                {
                    var compareDueDate = Date.Value.CompareTo(other.Date.Value);
                    if (compareDueDate != 0) return compareDueDate;
                }
                else if (Date.HasValue && Date.Value > DateTime.MinValue) return 1;
                else if (other.Date.HasValue && other.Date.Value > DateTime.MinValue) return -1;
                else return 0;

                return Id.CompareTo(other.Id);
            }

            public static int CompareTasksByDate(TestObject x, TestObject y)
            {
                if (x.Date.HasValue && y.Date.HasValue)
                {
                    return x.Date.Value.CompareTo(y.Date.Value);
                }
                else if (x.Date.HasValue) return 1;
                else if (y.Date.HasValue) return -1;

                return 0;
            }
            public static int CompareTasksByCompleted(TestObject x, TestObject y)
            {
                return x.Completed.CompareTo(y.Completed);
            }

            public static int CompareTasksByDescription(TestObject x, TestObject y)
            {
                return string.Compare(x.Description, y.Description, StringComparison.OrdinalIgnoreCase);
            }
        }

        const string SortByDescription = "Description";
        const string SortByDate = "Date";
        const string SortByCompleted = "Completed";

        [Fact]
        public void Sorting_TestResortingBehavior()
        {
            var random = new Random();

            var sortFunctions = new Dictionary<string, Comparison<TestObject>>()
            {
                { SortByDescription, TestObject.CompareTasksByDescription},
                { SortByDate, TestObject.CompareTasksByDate},
                { SortByCompleted, TestObject.CompareTasksByCompleted},

            };
            var cache = new SortableSubscribeableCache<TestObject>(sortFunctions);

            // Add a series of randomized objects that will require sorting
            // And can be compared that both lists are staying in sync
            var count = 0;
            var testObjects = GenerateListOfTestObjects();
            foreach (var to in testObjects)
            {
                cache.TryAdd(to.Id, to);
                count++;

                var desc_asc = cache.SortedBy(SortByDescription, false);
                var desc_desc = cache.SortedBy(SortByDescription, true);
                desc_desc.Reverse();
                Assert.True(desc_asc.SequenceEqual(desc_desc));
                Assert.Equal(count, desc_asc.Count);
                Assert.Equal(count, desc_desc.Count);
                Debug.WriteLine("TEST: List sorting is ok!");

                var date_asc = cache.SortedBy(SortByDate, false);
                var date_desc = cache.SortedBy(SortByDate, true);
                date_desc.Reverse();
                Assert.True(date_asc.SequenceEqual(date_desc));
                Assert.Equal(count, date_asc.Count);
                Assert.Equal(count, date_desc.Count);
                Debug.WriteLine("TEST: List sorting is ok!");

                var comp_asc = cache.SortedBy(SortByCompleted, false);
                var comp_desc = cache.SortedBy(SortByCompleted, true);
                comp_desc.Reverse();
                Assert.True(comp_asc.SequenceEqual(comp_desc));
                Assert.Equal(count, comp_asc.Count);
                Assert.Equal(count, comp_desc.Count);
                Debug.WriteLine("TEST: List sorting is ok!");
            }

            // Remove a random 5 objects from the lists and compare
            for (int i = 0; i < 5; i++)
            {
                var randomObject = testObjects.ElementAt(random.Next(testObjects.Count));
                var removed = cache.Remove(randomObject.Id);
                if (removed) count--;

                var desc_asc = cache.SortedBy(SortByDescription, false);
                var desc_desc = cache.SortedBy(SortByDescription, true);
                desc_desc.Reverse();
                Assert.True(desc_asc.SequenceEqual(desc_desc));
                Assert.Equal(count, desc_asc.Count);
                Assert.Equal(count, desc_desc.Count);
                Debug.WriteLine("TEST: List sorting is ok!");

                var date_asc = cache.SortedBy(SortByDate, false);
                var date_desc = cache.SortedBy(SortByDate, true);
                date_desc.Reverse();
                Assert.True(date_asc.SequenceEqual(date_desc));
                Assert.Equal(count, date_asc.Count);
                Assert.Equal(count, date_desc.Count);
                Debug.WriteLine("TEST: List sorting is ok!");

                var comp_asc = cache.SortedBy(SortByCompleted, false);
                var comp_desc = cache.SortedBy(SortByCompleted, true);
                comp_desc.Reverse();
                Assert.True(comp_asc.SequenceEqual(comp_desc));
                Assert.Equal(count, comp_asc.Count);
                Assert.Equal(count, comp_desc.Count);
                Debug.WriteLine("TEST: List sorting is ok!");
            }
        }

        List<TestObject> GenerateListOfTestObjects()
        {
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();

            var list = new List<TestObject>();

            for (int i = 0; i < 30; i++)
            {
                var rndLetter = letters[random.Next(26)];
                var rndDay = random.Next(28) + 1;
                var rndMonth = random.Next(12) + 1;
                var rndBool = random.Next(2) == 0 ? true : false;

                var obj = new TestObject(Guid.NewGuid().ToString(), rndLetter + " test description", DateTime.Parse($"2024-{rndMonth:00}-{rndDay:00}"), rndBool);

                list.Add(obj);
            }

            return list;
        }
    }
}