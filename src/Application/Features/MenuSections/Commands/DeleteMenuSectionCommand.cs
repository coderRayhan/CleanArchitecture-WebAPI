using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Constants;
using Application.Common.Events;
using Domain.Shared;
using MediatR;

namespace Application.Features.MenuSections.Commands;
public sealed record DeleteMenuSectionCommand(Guid Id)
    : ICacheInvalidatorCommand
{
    public string[] CacheKeys => [AppCacheKeys.Lookups];
}

internal sealed class DeleteMenuSectionCommandHandler(
    IApplicationDbContext dbContext,
    IPublisher publisher)
    : ICommandHandler<DeleteMenuSectionCommand>
{
    public async Task<Result> Handle(DeleteMenuSectionCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.MenuSections.FindAsync(request.Id, cancellationToken);

        if(entity is null)
            return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.MenuSections.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        // await publisher.Publish(new CacheInvalidationEvent() { CacheKey = AppCacheKeys.LookupDetails });
        
        return Result.Success();
    }
}
