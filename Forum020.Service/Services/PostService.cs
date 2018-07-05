using Forum020.Domain.UnitOfWork;
using Forum020.Service.CacheHelper;
using Forum020.Service.Interfaces;
using Forum020.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Forum020.Service.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _work;
        private readonly IDistributedCache _cache;

        public PostService(IUnitOfWork work, IDistributedCache cache)
        {
            _work = work;
            _cache = cache;
        }

        public async Task<BoardDTO> GetAllPostsForThread(string boardName, int threadId)
        {
            var board = await _cache.GetObjectAsync<BoardDTO>(RoutePaths.Posts(boardName, threadId));
            if (board == null)
            {
                board = await _work.PostRepository.GetAllPostsForThread(boardName, threadId);

                await _cache.SetObjectAsync(RoutePaths.Posts(boardName, threadId), board);
            }
            return board;
        }

        public async Task<BoardDTO> GetAllThreadsForBoard(string boardName)
        {
            var board = await _cache.GetObjectAsync<BoardDTO>(RoutePaths.Threads(boardName));
            if (board == null)
            {
                board = await _work.PostRepository.GetAllThreadsForBoard(boardName);

                await _cache.SetObjectAsync(RoutePaths.Threads(boardName), board);
            }
            return board;
        }

        public async Task<BoardDTO> GetLinkForPost(string boardName, int postId)
        {
            var board = await _cache.GetObjectAsync<BoardDTO>(postId.ToString());
            if (board == null)
            {
                board = await _work.PostRepository.GetPost(boardName, postId);

                if (board?.CurrentThread == null) return null;

                await _cache.SetObjectAsync(postId.ToString(), board);
            }

            return board;
        }

        public async Task<BoardDTO> PostPost(string boardName, int threadId, PostDTO post)
        {
            await _work.PostRepository.PostPost(boardName, threadId, post);
            await _work.SaveChangesAsync();

            var board = await _work.PostRepository.GetAllThreadsForBoard(boardName);
            await _cache.SetObjectAsync(RoutePaths.Threads(boardName), board);

            board = await _work.PostRepository.GetAllPostsForThread(boardName, threadId);
            await _cache.SetObjectAsync(RoutePaths.Posts(boardName, board.CurrentThread.Id), board);

            return board;
        }

        public async Task<BoardDTO> PostThread(string boardName, PostDTO thread)
        {
            var post = await _work.PostRepository.PostThread(boardName, thread);
            await _work.SaveChangesAsync();

            var board = await _work.PostRepository.GetAllThreadsForBoard(boardName);
            await _cache.SetObjectAsync(RoutePaths.Threads(boardName), board);

            board = await _work.PostRepository.GetAllPostsForThread(boardName, post.IdEffective);
            await _cache.SetObjectAsync(RoutePaths.Posts(boardName, board.CurrentThread.Id), board);

            return board;
        }
    }
}
