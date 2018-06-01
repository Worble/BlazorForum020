using Forum020.Domain.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Forum020.Domain.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveChangesAsync();
        IBoardRepository BoardRepository { get; }
        IPostRepository PostRepository { get; }
    }
}
