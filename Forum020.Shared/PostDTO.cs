using System;
using System.Collections.Generic;

namespace Forum020.Shared
{
    public class PostDTO : BaseDTO
    {
        public BoardDTO Board { get; set; }
        public int BoardId { get; set; }

        public IEnumerable<PostDTO> Posts { get; set; }

        public PostDTO Thread { get; set; }
        public int? ThreadId { get; set; }

        public bool IsOp { get; set; }
        public string Content { get; set; }
        public DateTime? BumpDate { get; set; }
        public bool IsArchived { get; set; }
        public string Image { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ImageChecksum { get; set; }
        public string UserIdentifier { get; set; }
    }
}
