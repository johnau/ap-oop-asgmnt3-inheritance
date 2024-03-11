using TaskManagerCore.Model;
using TaskManagerCore.Model.Dto;

namespace TaskManagerConsoleApp.ConsoleHelpers
{
    internal class TaskManagerDtoToGenericConverters
    {
        internal static List<Dictionary<string, object>> ConvertFolderDtosToGenericData(List<GetFolderDto> dtos)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (var d in dtos)
            {
                list.Add(ConvertFolderDtoToGenericData(d));
            }
            return list;
        }

        internal static Dictionary<string, object> ConvertFolderDtoToGenericData(GetFolderDto? dto)
        {
            if (dto == null) return new Dictionary<string, object>();
            return new Dictionary<string, object>
            {
                { "id", dto.Id },
                { UIData.PropertyName_Index, -1 },
                { UIData.PropertyName_Name, dto.Name },
                { UIData.PropertyName_TaskCount, dto.TaskIds.Count },
                { UIData.PropertyName_TaskIds, dto.TaskIds.ToArray() },
                { UIData.PropertyName_IncompleteCount, "~" },
            };
        }

        internal static CreateTaskDto ConvertGenericDataToTaskDto(Dictionary<string, object> taskData)
        {
            var type = TaskType.SINGLE;
            var folderId = "";
            var description = "";
            var notes = "";
            DateTime? dueDate = null;
            var interval = TimeInterval.None;

            if (taskData.TryGetValue(UIData.PropertyName_TaskType, out var _type))
                type = (TaskType)_type;            
            
            if (taskData.TryGetValue(UIData.PropertyName_InFolderId, out var _inFolderId))
                folderId = (string)_inFolderId;
            
            if (taskData.TryGetValue(UIData.PropertyName_Description, out var _description))
                description = (string)_description;
            else
                description = Guid.NewGuid().ToString();

            if (taskData.TryGetValue(UIData.PropertyName_Notes, out var _notes))
                notes = (string)_notes;

            if (taskData.TryGetValue(UIData.PropertyName_DueDate, out var _dueDateLong)
                && _dueDateLong is long && (long)_dueDateLong > 0L)
                dueDate = new DateTime((long)_dueDateLong);

            if (taskData.TryGetValue(UIData.PropertyName_Interval, out var _intervalHours))
                interval = (TimeInterval)(int)_intervalHours;

            return new CreateTaskDto(type, folderId, description, notes, dueDate, interval);
        }

        internal static List<Dictionary<string, object>> ConvertTaskDtosToGenericData(List<GetTaskDto> dtos)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (var d in dtos)
            {
                list.Add(ConvertTaskDtoToGenericData(d));
            }
            return list;
        }

        internal static Dictionary<string, object> ConvertTaskDtoToGenericData(GetTaskDto? dto)
        {
            if (dto == null) return new Dictionary<string, object>();

            return new Dictionary<string, object>
            {
                { "id", dto.Id },
                { UIData.PropertyName_Index, -1 },
                { UIData.PropertyName_TaskType, dto.Type },
                { UIData.PropertyName_Description, dto.Description },
                { UIData.PropertyName_Notes, dto.Notes },
                { UIData.PropertyName_Completed, dto.Completed },
                { UIData.PropertyName_DueDate, dto.DueDate.HasValue ? dto.DueDate.Value.Ticks : -1L },
                { UIData.PropertyName_Overdue, dto.Overdue},
            };
        }
    }
}
