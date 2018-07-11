using System;
using System.Collections.Generic;
using System.Text;

namespace Forum020.Shared
{
    public class BoardLinkDTO : BaseDTO
    {
        public string Name { get; set; }
        public string NameShort { get; set; }
        public PostDTO Post { get; set; }
    }
}
