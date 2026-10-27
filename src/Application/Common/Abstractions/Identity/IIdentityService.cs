using Application.Features.Identity.AppUsers.Commands;
using Domain.Shared;

namespace Application.Common.Abstractions.Identity;
public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

    Task<Result> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken = default);

    Task<Result<string>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken = default);

    Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);

    Task<Result> AddToRolesAsync(string userId, List<string> roles, CancellationToken cancellationToken);

    Task<Result> ChangePasswordAsync(
        string userId,
        string oldPassword, 
        string newPassword, 
        CancellationToken cancellationToken = default);
}
