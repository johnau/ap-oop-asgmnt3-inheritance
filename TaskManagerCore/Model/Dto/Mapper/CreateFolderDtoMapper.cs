namespace TaskManagerCore.Model.Dto.Mapper
{
    internal class CreateFolderDtoMapper : IDtoMapper<CreateFolderDto, TaskFolder>
    {
        public TaskFolder Map(CreateFolderDto dto)
        {
            return new TaskFolder(dto.Name);
        }
    }
}
