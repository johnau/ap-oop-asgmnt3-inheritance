using InMemoryCache;
using System;

namespace TaskManagerCore.Infrastructure.BinaryFile.Entity
{
    internal class EntityBase : IIdentifiable
    {
        public string Id { get; }

        protected EntityBase(string id = "")
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
