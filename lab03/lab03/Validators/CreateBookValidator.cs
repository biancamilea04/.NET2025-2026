using FluentValidation;
using lab03.Features;

namespace lab03.Validators;

public class CreateBookValidator : AbstractValidator<CreateBookRequest>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().NotNull().WithMessage("Title is required.");
        RuleFor(x => x.Author)
            .NotEmpty().NotNull().WithMessage("Author is required.");
        RuleFor(x => x.Year)
            .NotEmpty().NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Year)
                    .Matches(@"^\d{4}$").WithMessage("Year must be a valid four-digit number.");
            });
    }
}