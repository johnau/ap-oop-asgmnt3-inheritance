namespace TaskManagerConsoleApp.ConsoleHelpers
{
    internal class GeneralHelpers
    {
        internal static int ConvertToIntOrZero(string str)
        {
            if (int.TryParse(str, out int delIndex))
            {
                return delIndex;
            }
            return 0;
        }
    }
}
