using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace TaskManagerCore.Infrastructure.Sqlite.Entity
{
    internal class EntityBaseV2
    {
        [Key]
        public long Id { get; set; } // for auto generated id in database (we will store a record for every change to keep track of changes)
        public string GlobalId { get; set; }

        protected EntityBaseV2(string? globalId = "")
        {
            if (globalId != null && globalId != string.Empty)
            {
                GlobalId = globalId;
            } else
            {
                GlobalId = string.Empty;
            }
        }
    }
}
