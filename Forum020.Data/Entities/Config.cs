using System;
using System.Collections.Generic;
using System.Text;

namespace Forum020.Data.Entities
{
    public class Config : BaseEntity
    {
        public int MaximumThreadCount { get; set; }
        public int MaximumReplyCount { get; set; }
    }
}
