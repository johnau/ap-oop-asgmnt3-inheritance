using InMemoryCache; // not a great reference...

namespace TaskManagerCore.Infrastructure.Sqlite.Entity
{
    internal class EntityBase : IIdentifiable
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
