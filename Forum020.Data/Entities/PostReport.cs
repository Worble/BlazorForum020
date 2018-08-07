using System;
using System.Collections.Generic;
using System.Text;

namespace Forum020.Data.Entities
{
    public class PostReport : BaseEntity
    {
        public virtual Post Post { get; set; }
        public int PostId { get; set; }

        public virtual ReportType ReportType { get; set; }
        public int ReportTypeId { get; set; }

        public string AdditionalInformation { get; set; }
    }
}
