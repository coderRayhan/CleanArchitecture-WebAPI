using System.Text.Json.Serialization;
using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Domain.Entities.SuperAdmin;
using Domain.Shared;
using Mapster;

namespace Application.Features.MenuSectionSubItems.Commands;

public sealed record CreateMenuSectionSubItemCommand(
    Guid MenuSectionItemId,
    string Title,
    string Href,
    int SerialNo)
: ICacheInvalidatorCommand<Guid>
{
    [JsonIgnore]
    public string[] CacheKeys => [AppCacheKeys.MenuSectionItems];
}

internal sealed class CreateMenuSectionSubItemCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<CreateMenuSectionSubItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateMenuSectionSubItemCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<MenuSectionSubItem>();

        context.MenuSectionSubItems.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}