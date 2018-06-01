using Forum020.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forum020.Domain.Repositories.Interfaces
{
    public interface IBoardRepository
    {
        Task<IEnumerable<BoardDTO>> GetAllBoards();
    }
}
