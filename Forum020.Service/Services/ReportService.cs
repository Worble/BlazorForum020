using Forum020.Data.Entities;
using Forum020.Domain.UnitOfWork;
using Forum020.Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forum020.Service.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _work;

        public ReportService(IUnitOfWork work)
        {
            _work = work;
        }

        public async Task<IEnumerable<PostReport>> GetReports()
        {
            return await _work.ReportRepository.GetReports();
        }

        public async Task ReportPost(string boardName, int postId, int reportType)
        {
            await _work.PostRepository.ReportPost(boardName, postId, reportType);
            await _work.SaveChangesAsync();
        }
    }
}
