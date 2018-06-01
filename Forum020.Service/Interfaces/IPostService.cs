using Forum020.Shared;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Forum020.Service.Interfaces
{
    public interface IPostService
    {
        Task<BoardDTO> GetAllThreadsForBoard(string boardName);
        Task<BoardDTO> GetAllPostsForThread(string boardName, int thread);
        Task<BoardDTO> PostThread(string boardName, PostDTO thread);
        Task<BoardDTO> PostPost(string boardName, int threadId, PostDTO post);
    }
}
