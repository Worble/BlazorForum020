using System.Collections.Generic;

namespace Forum020.Data.Entities
{
    public class Board : BaseEntity
    {
        public string Name { get; set; }
        public string NameShort { get; set; }
        public virtual ICollection<Post> Threads { get; set; }
        public virtual Config Config { get; set; }
        public int ConfigId { get; set; }
    }
}
