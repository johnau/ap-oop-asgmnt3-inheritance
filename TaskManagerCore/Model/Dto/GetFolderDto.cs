
namespace TaskManagerCore.Model.Dto
{
    public class GetFolderDto
    {
        public string Id { get; }

        public string Name { get; }

        public List<string> TaskIds { get; }

        public GetFolderDto(string id, string name, List<string> taskIds)
        {
            Id = id;
            Name = name;
            TaskIds = taskIds;  
        }
    }
}
