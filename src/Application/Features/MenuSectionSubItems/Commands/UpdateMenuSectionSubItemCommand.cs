using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Constants;
using Domain.Shared;
using MediatR;

namespace Application.Features.MenuSectionSubItems.Commands
{
    public sealed record UpdateMenuSectionSubItemCommand(
        Guid Id,
        Guid MenuSectionItemId,
        string Title,
        string Href,
        int SerialNo)
        : ICacheInvalidatorCommand
    {
        public string[] CacheKeys => [AppCacheKeys.MenuSections];
    }

    internal sealed class UpdateMenuSectionSubItemCommandHandler(
        IApplicationDbContext dbContext,
        IPublisher publisher)
        : ICommandHandler<UpdateMenuSectionSubItemCommand>
    {
        public async Task<Result> Handle(UpdateMenuSectionSubItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await dbContext.MenuSectionSubItems.FindAsync(request.Id, cancellationToken);

            if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));
            
            entity.MenuSectionItemId = request.MenuSectionItemId;
            entity.Title = request.Title;
            entity.SerialNo = request.SerialNo;
            entity.Href = request.Href;

            await dbContext.SaveChangesAsync(cancellationToken);
            
            return Result.Success();
        }
    }
}
