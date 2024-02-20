namespace TaskManagerCore.Model.Dto.Mapper
{
    public class CreateTaskFolderDtoMapper : IDtoMapper<CreateFolderDto, TaskFolder>
    {
        public TaskFolder Map(CreateFolderDto dto)
        {
            return new TaskFolder(dto.Name);
        }
    }
}
