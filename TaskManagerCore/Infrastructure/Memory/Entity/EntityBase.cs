namespace TaskManagerCore.Infrastructure.Memory.Entity
{
    public class EntityBase
    {
        public string Id { get; }

        protected EntityBase(string? id = "")  
        {
            if (id != null && id != string.Empty)
            {
                Id = id;
            } else
            {
                Id = Guid.NewGuid().ToString();
            }
        }
    }
}
