using System.Text.Json.Serialization;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Identity;
using Domain.Shared;

namespace Application.Features.Identity.Roles.Commands;

public sealed record AddOrRemovePermissionCommand(
    string RoleId,
    List<string> Permissions)
    : ICacheInvalidatorCommand
{
    [JsonIgnore]
    public string[] CacheKeys => [AppCacheKeys.Roles];
}

internal sealed class AddOrRemovePermissionCommandHandler(
    IIdentityRoleService roleService) 
    : ICommandHandler<AddOrRemovePermissionCommand>
{
    public async Task<Result> Handle(AddOrRemovePermissionCommand request, CancellationToken cancellationToken)
    {
        return await roleService.AddorRemoveClaimsToRoleAsync(request.RoleId, request.Permissions, cancellationToken);
    }
}