using System.Collections.Generic;
using System.Threading.Tasks;
using Forum020.Domain.UnitOfWork;
using Forum020.Service.Interfaces;
using Forum020.Service.SessionHelper;
using Forum020.Shared;
using Microsoft.AspNetCore.Http;

namespace Forum020.Service.Services
{
    public class BoardService : IBoardService
    {
        private readonly IUnitOfWork _work;
        private readonly IHttpContextAccessor _contextAccessor;

        public BoardService(IUnitOfWork work, IHttpContextAccessor contextAccessor)
        {
            _work = work;
            _contextAccessor = contextAccessor;
        }

        public async Task<IEnumerable<BoardDTO>> GetAllBoards()
        {            
            if (!_contextAccessor.HttpContext.Session.TryGetObject(RoutePaths.Boards(), out IEnumerable<BoardDTO> boards))
            {
                boards = await _work.BoardRepository.GetAllBoards();

                _contextAccessor.HttpContext.Session.SetObject(RoutePaths.Boards(), boards);
            }
            return boards;
        }
    }
}
