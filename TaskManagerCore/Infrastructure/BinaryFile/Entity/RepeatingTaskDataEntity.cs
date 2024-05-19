using System;
using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class RepeatingTaskDataEntity : TaskDataEntity
    {
        public TimeInterval RepeatingInterval { get; set; }
        public int Repetitions { get; set; }

        //public DateTime StartFrom { get; set; }  // retain the start date?
        public RepeatingTaskDataEntity(string id = "")
            : base(id)
        { 
            DueDate = DateTime.MinValue;
            RepeatingInterval = TimeInterval.None;
            Repetitions = 0;
        }

        public override int CompareTo(TaskDataEntity other)
        {
            var baseResult = base.CompareTo(other);

            if (baseResult == 0 && other is RepeatingTaskDataEntity otherRepeatingTask)
            {
                var intervalCompare = ((int)RepeatingInterval).CompareTo((int)otherRepeatingTask.RepeatingInterval);
                if (intervalCompare != 0) 
                    return intervalCompare;

                return Repetitions.CompareTo(otherRepeatingTask.Repetitions);
            }

            return baseResult;
        }
    }
}
