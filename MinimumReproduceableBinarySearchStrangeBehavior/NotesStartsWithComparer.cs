namespace MinimumReproduceableBinarySearchStrangeBehavior
{
    internal class NotesStartsWithComparer : BinaryComparerBase<DataObject>
    {
        protected override int CompareNonNull(DataObject x, DataObject y)
        {
            var startsWithMatch = x.Notes.ToLower().StartsWith(y.Notes.ToLower());
            if (startsWithMatch) return 0;

            return DataObject.CompareByNotes(x, y);
        }
    }
}
