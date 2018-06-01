using Forum020.Service.Interfaces;
using Forum020.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Forum020.Server.Controllers
{
    [Route("api/boards")]
    public class BoardController : Controller
    {
        private readonly IBoardService _boardService;

        public BoardController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet]
        public async Task<IEnumerable<BoardDTO>> Get()
        {
            return await _boardService.GetAllBoards();
        }
    }
}
