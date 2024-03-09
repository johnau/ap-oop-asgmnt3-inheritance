namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers.Helper
{
    internal struct TaskDataTerminator
    {
        public const string IdTerminator = "!**##END##**!";
        public const string ClassNameTerminator = "!**##END##**!";
        public const string DescriptionTerminator = "!**##END##**!";
        public const string NotesTerminator = "!**##END##**!";
        public const bool CompletedTerminator = true;
        public static readonly long DueDateTerminator = long.MaxValue;
        public static readonly byte[] XDataTerminator = Enumerable.Repeat((byte)0xFF, sizeof(int) * 3).ToArray(); // 0xFF = 255 = 11111111 (max value)
    }
}
