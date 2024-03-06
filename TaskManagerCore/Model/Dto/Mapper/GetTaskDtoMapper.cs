namespace TaskManagerCore.Model.Dto.Mapper
{
    public class GetTaskDtoMapper : IDtoMapper<TaskData, GetTaskDto>
    {
        public GetTaskDto Map(TaskData o)
        {
            // Note:
            // `is` operator will return true for all types in the hierarchy
            // .GetType() == typeof(clazz) will only return true for exact class match
            // as such, the hierarchy must be checked in reverse order.
            // This is OK for this small linear hierarchy, but if expanded in two dimensions, GetType() == typeof()
            // will be more robust without a doubt.

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

        //public GetTaskDto Map(RepeatingTaskData o)
        //{
        //    var taskType = TaskType.REPEATING;
        //    var xData = new Dictionary<string, string>()
        //    {
        //        { "interval", o.RepeatingInterval.ToString() }
        //    };
            
        //    return new GetTaskDto(taskType, o.Id, o.Description, o.Notes, o.Completed, o.DueDate, o.Overdue);
        //}

        //public GetTaskDto Map(HabitualTaskData o)
        //{
        //    var taskType = TaskType.REPEATING_STREAK;
        //    var xData = new Dictionary<string, string>()
        //    {
        //        { "interval", o.RepeatingInterval.ToString() },
        //        { "streak", o.Streak+""}
        //    };
        //    return new GetTaskDto(taskType, o.Id, o.Description, o.Notes, o.Completed, o.DueDate, o.Overdue);
        //}
    }
}
