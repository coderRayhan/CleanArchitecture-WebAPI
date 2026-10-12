using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Identity;
using Domain.Shared;

namespace Application.Features.Identity.Roles.Commands;

public record DeleteRoleCommand(string Id) : ICacheInvalidatorCommand
{
    public string[] CacheKeys => [AppCacheKeys.Roles];
}

internal sealed class DeleteRoleCommandHandler(
    IIdentityRoleService roleService)
    : ICommandHandler<DeleteRoleCommand>
{
    public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        return await roleService.DeleteRoleAsync(request.Id, cancellationToken);

    }
}

