using System.Collections.Generic;
using System.Threading.Tasks;
using Forum020.Domain.UnitOfWork;
using Forum020.Service.Interfaces;
using Forum020.Service.CacheHelper;
using Forum020.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;

namespace Forum020.Service.Services
{
    public class BoardService : IBoardService
    {
        private readonly IUnitOfWork _work;
        private readonly IDistributedCache _cache;    

        public BoardService(IUnitOfWork work, IDistributedCache cache)
        {
            _work = work;
            _cache = cache;
        }

        public async Task<IEnumerable<BoardDTO>> GetAllBoards()
        {
            var boards = await _cache.GetObjectAsync<IEnumerable<BoardDTO>>(RoutePaths.Boards());
            if(boards == null)
            {
                boards = await _work.BoardRepository.GetAllBoards();

                await _cache.SetObjectAsync(RoutePaths.Boards(), boards);
            }
            return boards;
        }
    }
}
