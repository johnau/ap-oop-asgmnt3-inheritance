using System;
using System.Collections.Generic;
using System.Text;

namespace NaturalLanguageProcessor.Aggregates
{
#if NET8_0

    /// <summary>
    /// Represents generic information about a 'Task'
    /// </summary>
    /// <param name="Description"></param>
    /// <param name="HasTime"></param>
    /// <param name="OccursAt"></param>
    public record TaskInformation(string Description, bool HasTime, DateTime OccursAt);

#elif NETSTANDARD2_0

    public class TaskInformation
    {
        public string Description { get; }
        public bool HasTime { get; }
        public DateTime OccursAt { get; }

        public TaskInformation(string description, bool hasTime, DateTime occursAt)
        {
            Description = description;
            HasTime = hasTime;
            OccursAt = occursAt;
        }

        protected bool Equals(TaskInformation other)
        {
            return string.Equals(Description, other.Description) && HasTime == other.HasTime && OccursAt.Equals(other.OccursAt);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TaskInformation)obj);
        }
    }

#endif
}
