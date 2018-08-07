using Forum020.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum020.Admin.ViewModels
{
    public class ReportsViewModel
    {
        public IEnumerable<PostReport> Reports { get; set; }
    }
}
