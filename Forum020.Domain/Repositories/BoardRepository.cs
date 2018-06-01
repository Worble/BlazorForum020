using System.Collections.Generic;
using Forum020.Data;
using Forum020.Domain.Repositories.Interfaces;
using Forum020.Shared;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Forum020.Domain.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly ForumContext _context;

        public BoardRepository(ForumContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BoardDTO>> GetAllBoards()
        {
            return await _context.Boards.Select(e => new BoardDTO()
            {
                Id = e.Id,
                DateCreated = e.DateCreated,
                DateEdited = e.DateEdited,
                Name = e.Name,
                NameShort = e.NameShort
            }).ToListAsync();
        }
    }
}
