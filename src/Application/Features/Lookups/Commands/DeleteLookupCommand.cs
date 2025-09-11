using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Constants;
using Application.Common.Events;
using Domain.Shared;
using MediatR;

namespace Application.Features.Lookups.Commands;
public sealed record DeleteLookupCommand(Guid Id)
    : ICacheInvalidatorCommand
{
    public string[] CacheKeys => [AppCacheKeys.Lookups];
}

internal sealed class DeleteLookupCommandHandler(
    IApplicationDbContext dbContext,
    IPublisher publisher)
    : ICommandHandler<DeleteLookupCommand>
{
    public async Task<Result> Handle(DeleteLookupCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Lookups.FindAsync(request.Id, cancellationToken);

        if(entity is null)
            return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.Lookups.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        await publisher.Publish(new CacheInvalidationEvent() { CacheKey = AppCacheKeys.LookupDetails });
        
        return Result.Success();
    }
}
