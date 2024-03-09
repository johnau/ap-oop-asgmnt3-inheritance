namespace TaskManagerCore.Infrastructure.BinaryFile.FileHandlers.Helper
{
    internal struct TaskFolderTerminator
    {
        public const string IdTerminator = "!**##END##**!";
        public const string NameTerminator = "!**##END##**!";
        public static readonly string[] TaskIdsTerminator = new string[1] { "!**##END##**" };
    }
}
