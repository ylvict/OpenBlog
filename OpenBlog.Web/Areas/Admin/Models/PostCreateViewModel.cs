using FluentValidation;

namespace OpenBlog.Web.Areas.Admin.Models
{
    public class PostCreateViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
    }

    public class PostCreateViewModelValidator : PostEditViewModelValidator<PostCreateViewModel>
    {
    }

    public  abstract class PostEditViewModelValidator<T> : AbstractValidator<T>
        where T : PostCreateViewModel
    {
        protected PostEditViewModelValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title不能为空").MaximumLength(256).WithMessage("长度不能超过256");
            RuleFor(x => x.Content).NotEmpty().WithMessage("Content不能为空");
            RuleFor(x => x.Slug).Matches(@"^[0-9a-zA-Z_\s-]{1,}$").WithMessage("Slug不符合规则");
        }
    }

    public class PostEditViewModel : PostCreateViewModel
    {
        public string PostId { get; set; }
    }

    public class PostEditViewModelValidator : PostEditViewModelValidator<PostEditViewModel>
    {
    }
}