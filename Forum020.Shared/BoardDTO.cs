using System.Collections.Generic;

namespace Forum020.Shared
{
    public class BoardDTO : BaseDTO
    {
        public string Name { get; set; }
        public string NameShort { get; set; }
        public IEnumerable<PostDTO> Threads { get; set; }
        public PostDTO CurrentThread { get; set; }
    }
}
