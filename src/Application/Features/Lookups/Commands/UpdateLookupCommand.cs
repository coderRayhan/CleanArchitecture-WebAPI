using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Constants;
using Application.Common.Events;
using Domain.Shared;
using MediatR;

namespace Application.Features.Lookups.Commands
{
    public sealed record UpdateLookupCommand(
        Guid Id,
        string Name,
        string NameBN,
        string Code,
        string Description,
        bool Status,
        Guid? ParentId = null)
        : ICacheInvalidatorCommand
    {
        public string[] CacheKeys => [AppCacheKeys.Lookups];
    }

    internal sealed class UpdateLookupCommandHandler(
        IApplicationDbContext dbContext,
        IPublisher publisher)
        : ICommandHandler<UpdateLookupCommand>
    {
        public async Task<Result> Handle(UpdateLookupCommand request, CancellationToken cancellationToken)
        {
            var entity = await dbContext.Lookups.FindAsync(request.Id, cancellationToken);

            if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));

            bool previousStatus = entity.Status;

            entity.Name = request.Name;
            entity.NameBN = request.NameBN;
            entity.Code = request.Code;
            entity.Description = request.Description;
            entity.Status = request.Status;
            entity.ParentId = request.ParentId;

            await dbContext.SaveChangesAsync(cancellationToken);

            if (previousStatus != request.Status)
                await publisher.Publish(new CacheInvalidationEvent() { CacheKey = AppCacheKeys.LookupDetails });

            return Result.Success();
        }
    }
}
