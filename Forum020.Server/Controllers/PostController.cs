using Forum020.Server.Services.Interfaces;
using Forum020.Server.Validators;
using Forum020.Service.Interfaces;
using Forum020.Shared;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IImageService _imageService;
        private readonly INotificationsService _notificationsService;
        private readonly IHostingEnvironment _env;

        public PostController(IPostService postService, INotificationsService notificationsService, IHostingEnvironment env, IImageService imageService)
        {
            _postService = postService;
            _notificationsService = notificationsService;
            _imageService = imageService;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult<BoardDTO>> Get(string boardName)
        {
            return await _postService.GetAllThreadsForBoard(boardName);
        }

        [HttpGet("{threadId}")]
        public async Task<ActionResult<BoardDTO>> Get(string boardName, int threadId)
        {
            return await _postService.GetAllPostsForThread(boardName, threadId);
        }

        [HttpGet("get-link/{postId}")]
        public async Task<ActionResult<string>> GetLink(string boardName, int postId)
        {
            return await _postService.GetLinkForPost(boardName, postId);
        }

        [HttpPost]
        public async Task<ActionResult<BoardDTO>> PostThread(string boardName, [FromBody]PostDTO thread)
        {
            thread = SanitizePost(thread);
            var result = new ThreadValidator().Validate(thread);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            thread = _imageService.SaveImage(thread, _env, this.HttpContext.Request);
            return await _postService.PostThread(boardName, thread);
        }

        [HttpPost("{threadId}")]
        public async Task<ActionResult<BoardDTO>> PostPost(string boardName, int threadId, [FromBody]PostDTO post)
        {
            post = SanitizePost(post);
            var result = new PostValidator().Validate(post);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            if (!string.IsNullOrEmpty(post.Image))
            {
                if (await _imageService.IsImageUniqueToThread(post.Image, boardName, threadId))
                {
                    post = _imageService.SaveImage(post, _env, this.HttpContext.Request);
                }
                else
                {
                    return BadRequest();
                }
            } 
            
            var board = await _postService.PostPost(boardName, threadId, post);
            await _notificationsService.SendNotificationAsync(JsonConvert.SerializeObject(board));
            return board;
        }

        private PostDTO SanitizePost(PostDTO post)
        {
            return new PostDTO()
            {
                Content = post.Content,
                Image = post.Image
            };
        }
    }
}
