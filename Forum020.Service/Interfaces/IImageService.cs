using Forum020.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Forum020.Service.Interfaces
{
    public interface IImageService
    {
        PostDTO SaveImage(PostDTO post);

        Task<bool> IsImageUniqueToThread(string imageData, string boardName, int threadId);
    }
}
