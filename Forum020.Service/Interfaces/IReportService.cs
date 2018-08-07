using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Forum020.Data.Entities;

namespace Forum020.Service.Interfaces
{
    public interface IReportService
    {
        Task ReportPost(string boardName, int postId, int reportType);
        Task <IEnumerable<PostReport>> GetReports();
    }
}
