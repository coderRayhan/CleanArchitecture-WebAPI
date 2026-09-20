using Application.Common.Abstractions;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Constants;
using Application.Common.Events;
using Domain.Shared;
using MediatR;

namespace Application.Features.MenuSections.Commands
{
    public sealed record UpdateMenuSectionCommand(
        Guid Id,
        string Title,
        int SerialNo,
        string Href,
        string Icon,
        bool HasSubRoute)
        : ICacheInvalidatorCommand
    {
        public string[] CacheKeys => [AppCacheKeys.MenuSections];
    }

    internal sealed class UpdateMenuSectionCommandHandler(
        IApplicationDbContext dbContext,
        IPublisher publisher)
        : ICommandHandler<UpdateMenuSectionCommand>
    {
        public async Task<Result> Handle(UpdateMenuSectionCommand request, CancellationToken cancellationToken)
        {
            var entity = await dbContext.MenuSections.FindAsync(request.Id, cancellationToken);

            if (entity is null) return Result.Failure(Error.NotFound(nameof(entity), ErrorMessages.EntityNotFound));
            
            entity.Title = request.Title;
            entity.SerialNo = request.SerialNo;
            entity.Href = request.Href;
            entity.Icon = request.Icon;
            entity.HasSubRoute = request.HasSubRoute;

            await dbContext.SaveChangesAsync(cancellationToken);
            
            return Result.Success();
        }
    }
}
