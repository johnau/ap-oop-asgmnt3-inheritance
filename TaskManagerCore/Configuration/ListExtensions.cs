namespace TaskManagerCore.Configuration
{
    public static partial class ListExtensions
    {
        public static bool IsSortedBy<T>(this List<T> list, Comparison<T> comparison)
        {
            for (int i = 1; i < list.Count; i++)
            {
                if (comparison(list[i - 1], list[i]) > 0)
                    return false;
            }
            return true;
        }
    }
}
