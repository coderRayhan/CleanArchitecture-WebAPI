using System.Text.Json.Serialization;
using Application.Common.Abstractions.Caching;
using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Identity;
using Domain.Shared;

namespace Application.Features.Identity.Roles.Commands;

public sealed record CreateRoleCommand(
    string RoleName) : ICacheInvalidatorCommand<string>
{
    [JsonIgnore] public string[] CacheKeys => [AppCacheKeys.Roles];
}

internal sealed class CreateRoleCommandHandler(
    IIdentityRoleService roleService)
    : ICommandHandler<CreateRoleCommand, string>
{
    public async Task<Result<string>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var res = await roleService.CreateRoleAsync(request.RoleName, cancellationToken);
        return res;
    }
}