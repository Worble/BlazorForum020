using System;
using System.Collections.Generic;

namespace Forum020.Data.Entities
{
    public class Post : BaseEntity
    {
        public virtual Board Board { get; set; }
        public int BoardId { get; set; }

        public virtual Post Thread { get; set; }
        public int? ThreadId { get; set; }
        
        public int IdEffective { get; set; }
        public bool IsOp { get; set; }
        public string Content { get; set; }
        public DateTime? BumpDate { get; set; }
        public bool IsArchived { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ImageChecksum { get; set; }
        public string UserIdentifier { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
