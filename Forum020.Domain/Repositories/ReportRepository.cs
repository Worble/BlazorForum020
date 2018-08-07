using System.Collections.Generic;
using System.Threading.Tasks;
using Forum020.Data;
using Forum020.Data.Entities;
using Forum020.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Forum020.Domain.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ForumContext _context;

        public ReportRepository(ForumContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PostReport>> GetReports()
        {
            return await _context.PostReports
                .Include(e => e.Post)
                    .ThenInclude(e => e.Board)
                .Include(e => e.ReportType)
                .ToListAsync();
        }
    }
}
