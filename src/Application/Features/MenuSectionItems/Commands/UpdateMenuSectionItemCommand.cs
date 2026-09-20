using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Constants;
using Domain.Shared;
using MediatR;

namespace Application.Features.MenuSectionItems.Commands
{
    public sealed record UpdateMenuSectionItemCommand(
        Guid Id,
        Guid MenuSectionId,
        string Title,
        string Href,
        Boolean IsParent,
        int SerialNo)
        : ICacheInvalidatorCommand
    {
        public string[] CacheKeys => [AppCacheKeys.MenuSections];
    }

    internal sealed class UpdateMenuSectionItemCommandHandler(
        IApplicationDbContext dbContext,
        IPublisher publisher)
        : ICommandHandler<UpdateMenuSectionItemCommand>
    {
        public async Task<Result> Handle(UpdateMenuSectionItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await dbContext.MenuSectionItems.FindAsync(request.Id, cancellationToken);

            if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));
            
            entity.MenuSectionId = request.MenuSectionId;
            entity.Title = request.Title;
            entity.SerialNo = request.SerialNo;
            entity.Href = request.Href;
            entity.IsParent = request.IsParent;

            await dbContext.SaveChangesAsync(cancellationToken);
            
            return Result.Success();
        }
    }
}
