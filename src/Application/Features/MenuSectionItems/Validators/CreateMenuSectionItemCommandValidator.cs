using Application.Features.Lookups.Commands;
using Application.Features.MenuSectionItems.Commands;
using Application.Features.MenuSections.Commands;

namespace Application.Features.MenuSectionItems.Validators;
internal class CreateMenuSectionItemCommandValidator
    : AbstractValidator<CreateMenuSectionItemCommand>
{
    public CreateMenuSectionItemCommandValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(e => e.Href)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(e => e.MenuSectionId)
            .NotNull();
    }

    //private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    //{

    //}
}
