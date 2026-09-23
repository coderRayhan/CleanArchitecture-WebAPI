using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Domain.Shared;
using Mapster;

namespace Application.Features.MenuSection.Commands;

public sealed record CreateMenuSectionCommand(
        string Title,
        int SerialNo,
        string Href,
        string Icon,
        bool HasSubRoute
    ) : ICommand<Guid>;


internal sealed class CreateMenuSectionCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<CreateMenuSectionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateMenuSectionCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<Domain.Entities.SuperAdmin.MenuSection>();
        
        context.MenuSections.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success(entity.Id);
    }
}