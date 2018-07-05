using Forum020.Server.Validators;
using Forum020.Service.Interfaces;
using Forum020.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Forum020.Server.Controllers
{
    [Route("api/{boardName}")]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IImageService _imageService;
        private readonly IHostingEnvironment _env;
        private IConfiguration _config;

        public PostController(IPostService postService, IHostingEnvironment env, IImageService imageService, IConfiguration configuration)
        {
            _postService = postService;
            _imageService = imageService;
            _env = env;
            _config = configuration;
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
        public async Task<ActionResult<BoardDTO>> GetLink(string boardName, int postId)
        {
            return await _postService.GetLinkForPost(boardName, postId);
        }

        [HttpPost]
        public async Task<ActionResult<TokenDTO>> PostThread(string boardName, [FromBody]PostDTO thread)
        {
            thread = SanitizePost(thread);

            var result = new ThreadValidator().Validate(thread);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            bool createJwt = false;
            if (!User.Identity.IsAuthenticated)
            {
                CreateUser();
                createJwt = true;
            }

            thread.UserIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            thread = _imageService.SaveImage(thread);

            var board = await _postService.PostThread(boardName, thread);

            var response = new TokenDTO() { Board = board };
            if (createJwt)
            {
                response.Token = CreateToken();
            }

            return response;
        }

        [HttpPost("{threadId}")]
        public async Task<ActionResult<TokenDTO>> PostPost(string boardName, int threadId, [FromBody]PostDTO post)
        {
            post = SanitizePost(post);
            var result = new PostValidator().Validate(post);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            bool createJwt = false;
            if (!User.Identity.IsAuthenticated)
            {
                CreateUser();
                createJwt = true;
            }

            post.UserIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (!string.IsNullOrEmpty(post.Image))
            {
                if (await _imageService.IsImageUniqueToThread(post.Image, boardName, threadId))
                {
                    post = _imageService.SaveImage(post);
                }
                else
                {
                    return BadRequest();
                }
            } 
            
            var board = await _postService.PostPost(boardName, threadId, post);

            var response = new TokenDTO() { Board = board };
            if (createJwt)
            {
                response.Token = CreateToken();
            }

            return response;
        }

        [HttpPost("delete/{postId}")]
        public async Task<ActionResult<BoardDTO>> DeletePost(string boardName, int postId)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            var board = await _postService.DeletePost(boardName, postId, User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (board == null) return Forbid();

            return board;
        }

        #region helper methods

        private PostDTO SanitizePost(PostDTO post)
        {
            return new PostDTO()
            {
                Content = post.Content,
                Image = post.Image
            };
        }

        private void CreateUser()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            HttpContext.User = principal;
        }

        private string CreateToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "worble.xyz",
                audience: "worble.xyz",
                claims: User.Claims,
                expires: DateTime.Now.AddYears(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}
