using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Constants;
using Application.Common.Events;
using Domain.Shared;
using MediatR;

namespace Application.Features.MenuSectionItems.Commands;
public sealed record DeleteMenuSectionItemCommand(Guid Id)
    : ICacheInvalidatorCommand
{
    public string[] CacheKeys => [AppCacheKeys.Lookups];
}

internal sealed class DeleteMenuSectionItemCommandHandler(
    IApplicationDbContext dbContext,
    IPublisher publisher)
    : ICommandHandler<DeleteMenuSectionItemCommand>
{
    public async Task<Result> Handle(DeleteMenuSectionItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.MenuSectionItems.FindAsync(request.Id, cancellationToken);

        if(entity is null)
            return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.MenuSectionItems.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        // await publisher.Publish(new CacheInvalidationEvent() { CacheKey = AppCacheKeys.LookupDetails });
        
        return Result.Success();
    }
}
