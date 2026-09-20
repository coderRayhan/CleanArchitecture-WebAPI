using System.Text.Json.Serialization;
using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Domain.Entities.SuperAdmin;
using Domain.Shared;
using Mapster;

namespace Application.Features.MenuSections.Commands;

public sealed record CreateMenuSectionCommand(
    string Title,
    int SerialNo,
    string Href,
    string Icon,
    bool HasSubRoute)
: ICacheInvalidatorCommand<Guid>
{
    [JsonIgnore]
    public string[] CacheKeys => [AppCacheKeys.MenuSections];
}

internal sealed class CreateMenuSectionCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<CreateMenuSectionCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateMenuSectionCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<MenuSection>();

        context.MenuSections.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}