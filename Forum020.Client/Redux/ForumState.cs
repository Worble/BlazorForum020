using Forum020.Client.Shared;
using Forum020.Shared;
using System.Collections.Generic;

namespace Forum020.Client.Redux
{
    public class ForumState
    {
        public IEnumerable<BoardDTO> Boards { get; set; }
        public BoardDTO CurrentBoard { get; set; }
        public ThreadView ThreadViewType { get; set; }
        public string Content { get; set; }
        public string ErrorMessage { get; set; }
    }
}
