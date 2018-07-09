using Forum020.Data.Entities;
using Forum020.Shared;
using System.Threading.Tasks;

namespace Forum020.Domain.Repositories.Interfaces
{
    public interface IPostRepository
    {
        Task<BoardDTO> GetAllThreadsForBoard(string boardName);
        Task<BoardDTO> GetAllPostsForThread(string boardName, int thread);
        Task<Post> PostThread(string boardName, PostDTO thread);
        Task<Post> PostPost(string boardName, int threadId, PostDTO post);
        Task<bool> IsChecksumUnique(string checksum, string boardName, int threadId);
        Task<BoardDTO> GetPost(string boardName, int postId);
        Task<Post> DeletePost(string boardName, int postId);
        Task<Post> GetPostWithUserIdentifier(string boardName, int postId);
        Task<BoardDTO> DeleteImage(string boardName, int postId);
    }
}
