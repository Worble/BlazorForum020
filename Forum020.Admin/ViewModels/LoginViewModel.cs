using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum020.Admin.ViewModels
{
    public class LoginViewModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public bool Persist { get; set; }
        public string ReturnUrl { get; set; }
    }
}
