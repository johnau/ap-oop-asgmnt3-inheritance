namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class EntityBase
    {
        public string Id { get; }

        protected EntityBase(string? id = "")
        {
            if (id != null && id != string.Empty)
            {
                Id = id;
            }
            else
            {
                Id = Guid.NewGuid().ToString();
            }
        }
    }
}
