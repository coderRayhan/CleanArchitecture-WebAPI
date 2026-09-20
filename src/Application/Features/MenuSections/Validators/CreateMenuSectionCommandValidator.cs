using Application.Features.Lookups.Commands;
using Application.Features.MenuSections.Commands;

namespace Application.Features.MenuSections.Validators;
internal class CreateMenuSectionCommandValidator
    : AbstractValidator<CreateMenuSectionCommand>
{
    public CreateMenuSectionCommandValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(e => e.Href)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(e => e.Icon)
            .NotEmpty()
            .MaximumLength(100);
    }

    //private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    //{

    //}
}
