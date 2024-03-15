using TaskManagerCore.Infrastructure.BinaryFile.Entity;

namespace TaskManagerCore.Infrastructure.BinaryFile.QueryComparers
{
    internal class TaskFolderTaskCount_Comparer : QueryComparerBase<TaskFolderEntity>
    {
        private readonly Modifier _modifier;

        internal enum Modifier
        {
            EQUAL,
            MORE_THAN, 
            LESS_THAN,
        }
        public TaskFolderTaskCount_Comparer(Modifier modifier = Modifier.EQUAL)
        {
            _modifier = modifier;
        }

        protected override bool Equals(TaskFolderEntity searchCriteria, TaskFolderEntity t)
        {
            switch (_modifier)
            {
                case Modifier.EQUAL:
                    return t.TaskIds.Count == searchCriteria.TaskIds.Count;
                case Modifier.MORE_THAN:
                    return t.TaskIds.Count > searchCriteria.TaskIds.Count;
                case Modifier.LESS_THAN:
                    return t.TaskIds.Count < searchCriteria.TaskIds.Count;
                default: 
                    throw new ArgumentException("Unrecognized");
            }
            
        }
    }
}
