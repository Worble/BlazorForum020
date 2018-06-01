using System;
using System.Collections.Generic;

namespace Forum020.Shared
{
    public partial class BaseDTO
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateEdited { get; set; }
    }
}
