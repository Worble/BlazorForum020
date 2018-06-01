using Forum020.Shared;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forum020.Service.Interfaces
{
    public interface IBoardService
    {
        Task<IEnumerable<BoardDTO>> GetAllBoards();
    }
}
