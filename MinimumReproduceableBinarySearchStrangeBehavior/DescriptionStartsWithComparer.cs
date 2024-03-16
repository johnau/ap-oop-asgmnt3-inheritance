namespace MinimumReproduceableBinarySearchStrangeBehavior
{
    internal class DescriptionStartsWithComparer : BinaryComparerBase<DataObject>
    {
        protected override int CompareNonNull(DataObject x, DataObject y)
        {
            var startsWithMatch = x.Description.ToLower().StartsWith(y.Description.ToLower());
            if (startsWithMatch) return 0;

            return DataObject.CompareByDescription(x, y);
        }
    }
}
