namespace TaskManagerCore.Model.Dto.Mapper
{
    internal class GetFolderDtoMapper : IDtoMapper<TaskFolder, GetFolderDto>
    {
        public GetFolderDto Map(TaskFolder o)
        {
            return new GetFolderDto(o.Id, o.Name, o.TaskIds);
        }
    }
}
