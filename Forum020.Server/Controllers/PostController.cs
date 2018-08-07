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
using System.Net;
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
        private readonly IReportService _reportService;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;

        public PostController(IPostService postService, IHostingEnvironment env, IImageService imageService, IConfiguration configuration, IReportService reportService)
        {
            _postService = postService;
            _imageService = imageService;
            _env = env;
            _config = configuration;
            _reportService = reportService;
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
        public async Task<ActionResult<BoardLinkDTO>> GetLink(string boardName, int postId)
        {
            var board = await _postService.GetLinkForPost(boardName, postId);
            if (board == null) return NotFound();
            return board;
        }

        [HttpPost]
        public async Task<ActionResult<BoardDTO>> PostThread(string boardName, [FromBody]CreatePostDTO thread)
        {
            var result = new ThreadValidator().Validate(thread);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            if (!User.Identity.IsAuthenticated)
            {
                await CreateUser();
            }

            var threadPost = new PostDTO() { Content = thread.Content, Image = thread.Image };

            threadPost.UserIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            threadPost = _imageService.SaveImage(threadPost);

            return await _postService.PostThread(boardName, threadPost);
        }

        [HttpPost("{threadId}")]
        public async Task<ActionResult<BoardDTO>> PostPost(string boardName, int threadId, [FromBody]CreatePostDTO post)
        {
            var result = new PostValidator().Validate(post);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            if (!User.Identity.IsAuthenticated)
            {
                await CreateUser();
            }

            var postPost = new PostDTO() { Content = post.Content, Image = post.Image };

            postPost.UserIdentifier = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            postPost.ThreadId = threadId;

            if (!string.IsNullOrEmpty(postPost.Image))
            {
                if (await _imageService.IsImageUniqueToThread(postPost.Image, boardName, threadId))
                {
                    postPost = _imageService.SaveImage(postPost);
                }
                else
                {
                    return BadRequest("Duplicate Image Detected");
                }
            } 
            
            return await _postService.PostPost(boardName, threadId, postPost);
        }

        [HttpDelete("delete/{postId}")]
        public async Task<ActionResult<BoardDTO>> DeletePost(string boardName, int postId)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            if(await _postService.UserOwnsPost(boardName, postId, User.FindFirst(ClaimTypes.NameIdentifier).Value))
            { 
                var board = await _postService.DeletePost(boardName, postId);
                return board;
            }
            return Forbid();
        }

        [HttpDelete("delete-image/{postId}")]
        public async Task<ActionResult<BoardDTO>> DeleteImage(string boardName, int postId)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            if (!await _postService.UserOwnsPost(boardName, postId, User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Forbid();
            }

            if(!await _imageService.PostHasImage(boardName, postId))
            {
                return BadRequest("Post does not have image to delete");
            }

            await _imageService.DeleteImage(boardName, postId);
            var board = await _postService.DeleteImage(boardName, postId);
            return board;
        }
        
        [HttpPost("report/{postId}")]
        public async Task<IActionResult> ReportPost(string boardName, int postId, int reportType)
        {
            await _reportService.ReportPost(boardName, postId, reportType);
            return Ok();
        }

        #region helper methods

        private async Task CreateUser()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            HttpContext.User = principal;

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecurityKey"]));
            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //var token = new JwtSecurityToken(
            //    issuer: "worble.xyz",
            //    audience: "worble.xyz",
            //    claims: User.Claims,
            //    expires: DateTime.Now.AddYears(1),
            //    signingCredentials: creds);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddYears(1),
            });
        }

        //private string CreateToken()
        //{
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["SecurityKey"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: "worble.xyz",
        //        audience: "worble.xyz",
        //        claims: User.Claims,
        //        expires: DateTime.Now.AddYears(1),
        //        signingCredentials: creds);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        #endregion
    }
}
