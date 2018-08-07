using Forum020.Data;
using Forum020.Domain.Repositories;
using Forum020.Domain.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Forum020.Domain.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ForumContext _context;
        private bool _disposed = false;
        private IBoardRepository _boardRepository;
        private IPostRepository _postRepository;
        private IReportRepository _reportRepsitory;

        public UnitOfWork(ForumContext context)
        {
            _context = context;
        }

        public IBoardRepository BoardRepository => _boardRepository ?? (_boardRepository = new BoardRepository(_context));

        public IPostRepository PostRepository => _postRepository ?? (_postRepository = new PostRepository(_context));

        public IReportRepository ReportRepository => _reportRepsitory ?? (_reportRepsitory = new ReportRepository(_context));

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
