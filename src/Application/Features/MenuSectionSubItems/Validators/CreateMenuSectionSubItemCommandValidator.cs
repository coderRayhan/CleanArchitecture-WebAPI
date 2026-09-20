using Application.Features.Lookups.Commands;
using Application.Features.MenuSectionItems.Commands;
using Application.Features.MenuSections.Commands;
using Application.Features.MenuSectionSubItems.Commands;

namespace Application.Features.MenuSectionSubItems.Validators;
internal class CreateMenuSectionSubItemCommandValidator
    : AbstractValidator<CreateMenuSectionSubItemCommand>
{
    public CreateMenuSectionSubItemCommandValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(e => e.Href)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(e => e.MenuSectionItemId)
            .NotNull();
    }

    //private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    //{

    //}
}
