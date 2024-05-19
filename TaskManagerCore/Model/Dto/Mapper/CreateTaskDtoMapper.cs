using System;

namespace TaskManagerCore.Model.Dto.Mapper
{
    internal class CreateTaskDtoMapper : IDtoMapper<CreateTaskDto, TaskData>
    {
        public TaskData Map(CreateTaskDto dto)
        {
            switch (dto.TaskType)
            {
                case TaskType.SINGLE:
                    return new TaskData(dto.Description, dto.Notes, dto.DueDate);
                case TaskType.REPEATING:
                    return MapToRepeatingTaskDataObject(dto);
                case TaskType.REPEATING_STREAK:
                    return MapToHabitualTaskDataObject(dto);
                default:
                    throw new InvalidOperationException("Enum value not recognized");
            }
        }

        private RepeatingTaskData MapToRepeatingTaskDataObject(CreateTaskDto dto)
        {
            if (dto.RepeatInterval == null || dto.DueDate == null)
            {
                throw new ArgumentException("Expected arguments were not present");
            }
            return new RepeatingTaskData(
                dto.Description,
                dto.Notes,
                (DateTime)dto.DueDate,
                (TimeInterval)dto.RepeatInterval);
        }

        private HabitualTaskData MapToHabitualTaskDataObject(CreateTaskDto dto)
        {
            if (dto.RepeatInterval == null || dto.DueDate == null)
            {
                throw new ArgumentException("Expected arguments were not present");
            }
            return new HabitualTaskData(
                dto.Description,
                dto.Notes,
                (DateTime)dto.DueDate,
                (TimeInterval)dto.RepeatInterval);
        }
    }
}
