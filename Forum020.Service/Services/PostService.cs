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

        public async Task<BoardDTO> DeleteImage(string boardName, int postId)
        {
            var post = await _work.PostRepository.DeleteImage(boardName, postId);
            if (string.IsNullOrEmpty(post.CurrentThread.Content))
            {
                return await DeletePost(boardName, postId);
            }
            await _work.SaveChangesAsync();
            await _cache.SetObjectAsync(RoutePaths.SinglePost(boardName, postId), post);

            var board = await _work.PostRepository.GetAllPostsForThread(boardName, post.CurrentThread.ThreadId.Value);
            await _cache.SetObjectAsync(RoutePaths.PostsRoute(board.NameShort, board.CurrentThread.Id), board);

            return board;
        }

        public async Task<BoardDTO> DeletePost(string boardName, int postId)
        {
            var post = await _work.PostRepository.DeletePost(boardName, postId);
            await _work.SaveChangesAsync();
            await _cache.RemoveAsync(RoutePaths.SinglePost(boardName, postId));

            var board = await _work.PostRepository.GetAllPostsForThread(boardName, post.Thread.IdEffective);
            await _cache.SetObjectAsync(RoutePaths.PostsRoute(board.NameShort, board.CurrentThread.Id), board);

            return board;
        }

        public async Task<BoardDTO> GetAllPostsForThread(string boardName, int threadId)
        {
            var board = await _cache.GetObjectAsync<BoardDTO>(RoutePaths.PostsRoute(boardName, threadId));
            if (board == null)
            {
                board = await _work.PostRepository.GetAllPostsForThread(boardName, threadId);

                await _cache.SetObjectAsync(RoutePaths.PostsRoute(boardName, threadId), board);
            }
            return board;
        }

        public async Task<BoardDTO> GetAllThreadsForBoard(string boardName)
        {
            var board = await _cache.GetObjectAsync<BoardDTO>(RoutePaths.ThreadsRoute(boardName));
            if (board == null)
            {
                board = await _work.PostRepository.GetAllThreadsForBoard(boardName);

                await _cache.SetObjectAsync(RoutePaths.ThreadsRoute(boardName), board);
            }
            return board;
        }

        public async Task<BoardLinkDTO> GetLinkForPost(string boardName, int postId)
        {
            var board = await _cache.GetObjectAsync<BoardLinkDTO>(RoutePaths.SinglePost(boardName, postId));
            if (board == null)
            {
                board = await _work.PostRepository.GetPost(boardName, postId);

                if (board?.Post == null) return null;

                await _cache.SetObjectAsync(RoutePaths.SinglePost(boardName, postId), board);
            }

            return board;
        }

        public async Task<BoardDTO> PostPost(string boardName, int threadId, PostDTO post)
        {
            await _work.PostRepository.PostPost(boardName, threadId, post);
            await _work.SaveChangesAsync();

            var board = await _work.PostRepository.GetAllThreadsForBoard(boardName);
            await _cache.SetObjectAsync(RoutePaths.ThreadsRoute(boardName), board);

            board = await _work.PostRepository.GetAllPostsForThread(boardName, threadId);
            await _cache.SetObjectAsync(RoutePaths.PostsRoute(boardName, board.CurrentThread.Id), board);

            return board;
        }

        public async Task<BoardDTO> PostThread(string boardName, PostDTO thread)
        {
            var post = await _work.PostRepository.PostThread(boardName, thread);
            await _work.SaveChangesAsync();

            var board = await _work.PostRepository.GetAllThreadsForBoard(boardName);
            await _cache.SetObjectAsync(RoutePaths.ThreadsRoute(boardName), board);

            board = await _work.PostRepository.GetAllPostsForThread(boardName, post.IdEffective);
            await _cache.SetObjectAsync(RoutePaths.PostsRoute(boardName, board.CurrentThread.Id), board);

            return board;
        }

        public async Task<bool> UserOwnsPost(string boardName, int postId, string userIdentifier)
        {
            var post = await _work.PostRepository.GetPostWithUserIdentifier(boardName, postId);
            
            if (post == null ||
                post.UserIdentifier != userIdentifier ||
                post.IsOp)
            {
                return false;
            }
            return true;
        }
    }
}
