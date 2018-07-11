using FluentValidation;
using Forum020.Shared;

namespace Forum020.Server.Validators
{
    public class ThreadValidator : AbstractValidator<CreatePostDTO>
    {
        public ThreadValidator()
        {
            RuleFor(post => post.Image).NotNull().NotEmpty().Matches(@"data:image/(?<type>.+?);base64,(?<data>.+)");
        }
    }
}
