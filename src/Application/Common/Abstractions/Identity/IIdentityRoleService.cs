using Application.Features.Identity.Roles.Models;
using Domain.Shared;

namespace Application.Common.Abstractions.Identity;
public interface IIdentityRoleService
{
    Task<Result<string>> CreateRoleAsync(string name, CancellationToken cancellationToken);

    Task<Result> UpdateRoleAsync(string id, string name, CancellationToken cancellationToken);

    Task<Result<string>> GetRoleAsync(string id, CancellationToken cancellationToken);
    
    Task<Result> DeleteRoleAsync(string id, CancellationToken cancellationToken);

    Task<Result> AddorRemoveClaimsToRoleAsync(string id, List<string> permissions, CancellationToken cancellationToken);
    Task<Result<RoleModel>> GetRolePermissionsAsync(string id, CancellationToken cancellationToken = default);
}
