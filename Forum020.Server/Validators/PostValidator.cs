using FluentValidation;
using Forum020.Shared;

namespace Forum020.Server.Validators
{
    public class PostValidator : AbstractValidator<CreatePostDTO>
    {
        public PostValidator()
        {
            RuleFor(post => post.Content).Must((post, content) => string.IsNullOrEmpty(post.Image) ? !string.IsNullOrEmpty(content) : true);
            RuleFor(post => post.Image).Must((post, image) => string.IsNullOrEmpty(post.Content) ? !string.IsNullOrEmpty(image) : true);
            RuleFor(post => post.Image).Matches(@"data:image/(?<type>.+?);base64,(?<data>.+)").When(e => !string.IsNullOrEmpty(e.Image));
        }
    }
}
