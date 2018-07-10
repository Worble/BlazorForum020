using Forum020.Shared;
using System.Threading.Tasks;

namespace Forum020.Service.Interfaces
{
    public interface IImageService
    {
        PostDTO SaveImage(PostDTO post);

        Task<bool> IsImageUniqueToThread(string imageData, string boardName, int threadId);
        Task DeleteImage(string boardName, int postId);
        Task<bool> PostHasImage(string boardName, int postId);
    }
}
