namespace TaskManagerCore.Model.Dto
{
    public class CreateFolderDto
    {
        public string Name { get; }

        public CreateFolderDto(string name)
        {
            Name = name;
        }
    }
}
