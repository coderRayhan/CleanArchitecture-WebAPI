using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Domain.Entities;
using Domain.Shared;
using Mapster;
using System.Text.Json.Serialization;
using Application.Common.Abstractions.Caching;

namespace Application.Features.Lookups.Commands;
public sealed record CreateLookupCommand(
    string Name,
    string NameBN,
    string Code,
    string Description,
    bool Status,
    Guid? ParentId = null)
    : ICacheInvalidatorCommand<Guid>
{
    [JsonIgnore]
    public string[] CacheKeys => [AppCacheKeys.Lookups];
}

internal sealed class CreateLookupCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateLookupCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateLookupCommand request, CancellationToken cancellationToken)
    {
        var entity = request.Adapt<Lookup>();

        dbContext.Lookups.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(entity.Id);
    }
}
