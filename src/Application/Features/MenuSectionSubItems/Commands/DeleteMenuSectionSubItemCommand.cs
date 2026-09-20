using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Constants;
using Application.Common.Events;
using Domain.Shared;
using MediatR;

namespace Application.Features.MenuSectionSubItems.Commands;
public sealed record DeleteMenuSectionSubItemCommand(Guid Id)
    : ICacheInvalidatorCommand
{
    public string[] CacheKeys => [AppCacheKeys.Lookups];
}

internal sealed class DeleteMenuSectionSubItemCommandHandler(
    IApplicationDbContext dbContext)
    : ICommandHandler<DeleteMenuSectionSubItemCommand>
{
    public async Task<Result> Handle(DeleteMenuSectionSubItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.MenuSectionSubItems.FindAsync(request.Id, cancellationToken);

        if(entity is null)
            return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

        dbContext.MenuSectionSubItems.Remove(entity);

        await dbContext.SaveChangesAsync(cancellationToken);

        // await publisher.Publish(new CacheInvalidationEvent() { CacheKey = AppCacheKeys.LookupDetails });
        
        return Result.Success();
    }
}
