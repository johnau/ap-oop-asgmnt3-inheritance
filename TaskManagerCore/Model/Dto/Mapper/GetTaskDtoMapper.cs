using System.Collections.Generic;

namespace TaskManagerCore.Model.Dto.Mapper
{
    internal class GetTaskDtoMapper : IDtoMapper<TaskData, GetTaskDto>
    {
        public GetTaskDto Map(TaskData o)
        {
            // testing out using `is` rather than the type check with GetType() and typeof()
            // Order is important with `is` due to matching all inherited

            //if (o.GetType() == typeof(HabitualTaskData))
            if (o is HabitualTaskData habitual)
            {
                var taskType = TaskType.REPEATING_STREAK;
                var xData = new Dictionary<string, string>()
                    {
                        { "interval", habitual.RepeatingInterval.ToString() },
                        { "streak", habitual.Streak+""}
                    };
                return new GetTaskDto(taskType, o.Id, o.Description, o.Notes, o.Completed, o.DueDate, o.Overdue)
                    .WithExtraData(xData);
            }
            //else if (o.GetType() == typeof(RepeatingTaskData))
            else if (o is RepeatingTaskData repeating)
            {
                var taskType = TaskType.REPEATING;
                var xData = new Dictionary<string, string>()
                    {
                        { "interval", repeating.RepeatingInterval.ToString() }
                    };
                return new GetTaskDto(taskType, o.Id, o.Description, o.Notes, o.Completed, o.DueDate, o.Overdue)
                        .WithExtraData(xData);
            }
            // else if (o.GetType() == typeof(TaskData)) {} // explicit check all and error on else
            //else { throw new ArgumentException("Unrecognized type"); }
            else
            {
                var taskType = TaskType.SINGLE;
                return new GetTaskDto(taskType, o.Id, o.Description, o.Notes, o.Completed, o.DueDate, o.Overdue);
            }
        }
    }
}
