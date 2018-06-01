using System;

namespace Forum020.Data
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateEdited { get; set; }
    }
}
