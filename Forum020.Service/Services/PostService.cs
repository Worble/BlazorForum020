using Forum020.Domain.UnitOfWork;
using Forum020.Service.Interfaces;
using Forum020.Service.SessionHelper;
using Forum020.Shared;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Forum020.Service.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _work;
        private readonly IHttpContextAccessor _contextAccessor;

        public PostService(IUnitOfWork work, IHttpContextAccessor contextAccessor)
        {
            _work = work;
            _contextAccessor = contextAccessor;
        }

        public async Task<BoardDTO> GetAllPostsForThread(string boardName, int threadId)
        {
            if (!_contextAccessor.HttpContext.Session.TryGetObject(RoutePaths.Posts(boardName, threadId), out BoardDTO board))
            {
                board = await _work.PostRepository.GetAllPostsForThread(boardName, threadId);

                _contextAccessor.HttpContext.Session.SetObject(RoutePaths.Posts(boardName, threadId), board);
            }
            return board;
        }

        public async Task<BoardDTO> GetAllThreadsForBoard(string boardName)
        {
            if (!_contextAccessor.HttpContext.Session.TryGetObject(RoutePaths.Threads(boardName), out BoardDTO board))
            {
                board = await _work.PostRepository.GetAllThreadsForBoard(boardName);

                _contextAccessor.HttpContext.Session.SetObject(RoutePaths.Threads(boardName), board);
            }
            return board;
        }

        public async Task<string> GetLinkForPost(string boardName, int postId)
        {
            if (!_contextAccessor.HttpContext.Session.TryGetObject(postId.ToString(), out string url))
            {
                var board = await _work.PostRepository.GetPost(boardName, postId);
                if(board?.CurrentThread == null) return string.Empty;
                url = board.NameShort + "/" + 
                    (board.CurrentThread.IsOp ? 
                    board.CurrentThread.Id.ToString() : 
                    board.CurrentThread.ThreadId.ToString() + "#" + board.CurrentThread.Id.ToString());

                _contextAccessor.HttpContext.Session.SetObject(postId.ToString(), url);
            }

            return url;
        }

        public async Task<BoardDTO> PostPost(string boardName, int threadId, PostDTO post)
        {
            await _work.PostRepository.PostPost(boardName, threadId, post);
            await _work.SaveChangesAsync();

            var board = await _work.PostRepository.GetAllThreadsForBoard(boardName);
            _contextAccessor.HttpContext.Session.SetObject(RoutePaths.Threads(boardName), board);

            board = await _work.PostRepository.GetAllPostsForThread(boardName, threadId);
            _contextAccessor.HttpContext.Session.SetObject(RoutePaths.Posts(boardName, board.CurrentThread.Id), board);

            return board;
        }

        public async Task<BoardDTO> PostThread(string boardName, PostDTO thread)
        {
            var post = await _work.PostRepository.PostThread(boardName, thread);
            await _work.SaveChangesAsync();

            var board = await _work.PostRepository.GetAllThreadsForBoard(boardName);
            _contextAccessor.HttpContext.Session.SetObject(RoutePaths.Threads(boardName), board);

            board = await _work.PostRepository.GetAllPostsForThread(boardName, post.IdEffective);
            _contextAccessor.HttpContext.Session.SetObject(RoutePaths.Posts(boardName, board.CurrentThread.Id), board);

            return board;
        }
    }
}
