namespace TaskManagerCore.Model.Dto.Mapper
{
    public class CreateFolderDtoMapper : IDtoMapper<CreateFolderDto, TaskFolder>
    {
        public TaskFolder Map(CreateFolderDto dto)
        {
            return new TaskFolder(dto.Name);
        }
    }
}
