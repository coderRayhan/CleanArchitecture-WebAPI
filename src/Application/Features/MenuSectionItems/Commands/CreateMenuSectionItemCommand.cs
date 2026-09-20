using System.Text.Json.Serialization;
using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Domain.Entities.SuperAdmin;
using Domain.Shared;
using Mapster;

namespace Application.Features.MenuSectionItems.Commands;

public sealed record CreateMenuSectionItemCommand(
    Guid MenuSectionId,
    string Title,
    string Href,
    Boolean IsParent,
    int SerialNo)
: ICacheInvalidatorCommand<Guid>
{
    [JsonIgnore]
    public string[] CacheKeys => [AppCacheKeys.MenuSectionItems];
}

internal sealed class CreateMenuSectionItemCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<CreateMenuSectionItemCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateMenuSectionItemCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<MenuSectionItem>();

        context.MenuSectionItems.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}