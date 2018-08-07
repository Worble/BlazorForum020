using System.Collections.Generic;
using System.Threading.Tasks;
using Forum020.Data.Entities;

namespace Forum020.Domain.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<PostReport>> GetReports();
    }
}
