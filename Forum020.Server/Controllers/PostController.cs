using Forum020.Server.Services.Interfaces;
using Forum020.Service.Interfaces;
using Forum020.Shared;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Forum020.Server.Controllers
{
    [Route("api/{boardName}")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly INotificationsService _notificationsService;

        public PostController(IPostService postService, INotificationsService notificationsService)
        {
            _postService = postService;
            _notificationsService = notificationsService;
        }

        [HttpGet]
        public async Task<BoardDTO> Get(string boardName)
        {
            return await _postService.GetAllThreadsForBoard(boardName);
        }

        [HttpGet("{threadId}")]
        public async Task<BoardDTO> Get(string boardName, int threadId)
        {
            return await _postService.GetAllPostsForThread(boardName, threadId);
        }

        [HttpPost]
        public async Task<BoardDTO> PostThread(string boardName, [FromBody]PostDTO thread)
        {
            return await _postService.PostThread(boardName, thread);
        }

        [HttpPost("{threadId}")]
        public async Task<BoardDTO> PostPost(string boardName, int threadId, [FromBody]PostDTO post)
        {
            var board = await _postService.PostPost(boardName, threadId, post);
            await _notificationsService.SendNotificationAsync(JsonConvert.SerializeObject(board));
            return board;
        }
    }
}
