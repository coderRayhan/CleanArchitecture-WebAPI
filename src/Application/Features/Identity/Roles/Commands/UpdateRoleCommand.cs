using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Identity;
using Domain.Shared;

namespace Application.Features.Identity.Roles.Commands;

public sealed record UpdateRoleCommand(Guid Id, string roleName)
: ICacheInvalidatorCommand
{
    public string[] CacheKeys => [AppCacheKeys.Roles];
}

internal sealed class UpdateRoleCommandHandler(
    IIdentityRoleService roleService) 
    : ICommandHandler<UpdateRoleCommand>
{
    public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleService.GetRoleAsync(request.Id.ToString(),  cancellationToken);

        if (role == null) return Result.Failure(Error.NotFound(nameof(role), "Role not found"));
        
        await roleService.UpdateRoleAsync(request.Id.ToString(), request.roleName, cancellationToken);
        return Result.Success();
    }
}