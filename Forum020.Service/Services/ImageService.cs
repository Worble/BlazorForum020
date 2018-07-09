using Forum020.Domain.UnitOfWork;
using Forum020.Service.Interfaces;
using Forum020.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.Primitives;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Forum020.Service.Services
{
    public class ImageService : IImageService
    {
        private readonly IUnitOfWork _work;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ImageService(IUnitOfWork work, IHttpContextAccessor httpContextAccessor, IHostingEnvironment env)
        {
            _work = work;
            _contextAccessor = httpContextAccessor;
            _hostingEnvironment = env;
        }

        public PostDTO SaveImage(PostDTO post)
        {
            var regex = Regex.Match(post.Image, @"data:image/(?<type>.+?);base64,(?<data>.+)");

            if (!regex.Success) return post;

            var base64 = regex.Groups["data"].Value;
            var binData = Convert.FromBase64String(base64);

            if (binData.Length / 1048576 > 3)
            {
                throw new Exception();
            }

            var image = Image.Load(binData);
            var thumbnail = image.Clone();

            string name = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();

            string imageName = name + "." + Image.DetectFormat(binData).Name.ToLower();
            string thumbnailName = name + ".jpeg";

            string imagePath = "/Images/";
            string thumbnailPath = "/Thumbnails/";

            string localImagePath = Path.Combine(_hostingEnvironment.WebRootPath + imagePath, imageName);
            string localThumbnailPath = Path.Combine(_hostingEnvironment.WebRootPath + thumbnailPath, thumbnailName);

            var req = _contextAccessor.HttpContext.Request;

            string webImagePath = req.Scheme + "://" + req.Host + req.PathBase + imagePath + imageName;
            string webThumbnailPath = req.Scheme + "://" + req.Host + req.PathBase + thumbnailPath + thumbnailName;

            thumbnail.Mutate(i => i.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Max,
                    Size = new Size() { Width = 100, Height = 100 }
                })
            );

            if (!File.Exists(localImagePath) || !File.Exists(localThumbnailPath))
            {
                FileInfo file = new FileInfo(localImagePath);
                file.Directory?.Create();
                file = new FileInfo(localThumbnailPath);
                file.Directory?.Create();
                image.Save(localImagePath);
                thumbnail.Save(localThumbnailPath, new JpegEncoder());
            }

            post.ImageUrl = webImagePath;
            post.ThumbnailUrl = webThumbnailPath;

            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(binData);
                post.ImageChecksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }

            return post;
        }

        public async Task<bool> IsImageUniqueToThread(string imageData, string boardName, int threadId)
        {
            var regex = Regex.Match(imageData, @"data:image/(?<type>.+?);base64,(?<data>.+)");
            var base64 = regex.Groups["data"].Value;
            var binData = Convert.FromBase64String(base64);
            string checksum;

            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(binData);
                checksum = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }

            return await _work.PostRepository.IsChecksumUnique(checksum, boardName, threadId);
        }

        public async Task DeleteImage(string boardName, int postId)
        {
            string imagePath = "/Images/";
            string thumbnailPath = "/Thumbnails/";

            var post = await _work.PostRepository.GetPost(boardName, postId);

            var thumbnailArray = post.CurrentThread.ThumbnailUrl.Split('/');
            var thumbnailName = thumbnailArray[thumbnailArray.Length-1];
            string localThumbnail = Path.Combine(_hostingEnvironment.WebRootPath + thumbnailPath, thumbnailName);
            File.Delete(localThumbnail);

            var imageArray = post.CurrentThread.ImageUrl.Split('/');
            var imageName = thumbnailArray[imageArray.Length-1];
            string localImagePath = Path.Combine(_hostingEnvironment.WebRootPath + imagePath, imageName);
            File.Delete(localImagePath);
        }
    }
}

