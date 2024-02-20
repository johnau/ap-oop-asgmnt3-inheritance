namespace TaskManagerCore.Model.Dto.Mapper
{
    public class GetTaskDtoMapper : IDtoMapper<TaskData, GetTaskDto>
    {
        public GetTaskDto Map(TaskData o)
        {
            if (o is HabitualTaskData)
            {
                var taskType = TaskType.REPEATING_STREAK;
                var xData = new Dictionary<string, string>()
                    {
                        { "interval", ((HabitualTaskData)o).RepeatingInterval.ToString() },
                        { "streak", ((HabitualTaskData)o).Streak+""}
                    };
                return new GetTaskDto(taskType, o.Id, o.Description, o.Notes, o.Completed, o.DueDate, o.Overdue);
            }
            else if (o is RepeatingTaskData)
            {
                var taskType = TaskType.REPEATING;
                var xData = new Dictionary<string, string>()
                    {
                        { "interval", ((RepeatingTaskData)o).RepeatingInterval.ToString() }
                    };

                return new GetTaskDto(taskType, o.Id, o.Description, o.Notes, o.Completed, o.DueDate, o.Overdue);
            }
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
