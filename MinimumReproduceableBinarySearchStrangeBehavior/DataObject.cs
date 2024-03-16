namespace MinimumReproduceableBinarySearchStrangeBehavior
{
    internal class DataObject
    {
        public string Description { get; set; }
        public string Notes { get; set; }

        public DataObject(string description, string notes)
        {
            Description = description;
            Notes = notes;
        }

        public static int CompareByDescription(DataObject x, DataObject y)
        {
            //return string.Compare(x.Description, y.Description, StringComparison.OrdinalIgnoreCase);
            return StringComparer.OrdinalIgnoreCase.Compare(x.Description, y.Description);
        }
        public static int CompareByNotes(DataObject x, DataObject y)
        {
            //return string.Compare(x.Notes, y.Notes, StringComparison.OrdinalIgnoreCase);
            return StringComparer.OrdinalIgnoreCase.Compare(x.Notes, y.Notes);
        }

        public override string ToString()
        {
            return $"Descr=\t{Description, 25} \t Notes=\t{Notes}";
        }
    }
}
