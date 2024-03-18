﻿using TaskManagerCore.Model;

namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class RepeatingTaskDataEntity : TaskDataEntity
    {
        //public new DateTime DueDate { get; set; }
        public TimeInterval RepeatingInterval { get; set; }
        public int Repetitions { get; set; }

        //public DateTime StartFrom { get; set; }  // retain the start date?
        public RepeatingTaskDataEntity(string? id = "")
            : base(id)
        {
        }
    }
}
