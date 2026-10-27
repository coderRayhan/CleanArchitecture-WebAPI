using Application.Common.Abstractions.Identity;
using Application.Common.Constants;
using Application.Features.Identity.AppUsers.Commands;
using Domain.Shared;
using Infrastructure.Identity.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity.Services;
public class IdentityService(
    UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService,
    IdentityContext identityContext) : IIdentityService
{
    public async Task<Result> AddToRolesAsync(string userId, List<string> roles, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), "User not found"));

        var result = await userManager.AddToRolesAsync(user, roles);

        return result.Succeeded ? Result.Success() :
            Result.Failure(Error.Unauthorized(nameof(ErrorType.Unauthorized), string.Empty));
    }

    public async Task<Result> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken = default)
    {
        var user = userManager.Users.SingleOrDefault(u => u.Id == userId);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), "User not found"));

        var principle = await userClaimsPrincipalFactory.CreateAsync(user);

        var result = await authorizationService.AuthorizeAsync(principle, policyName);

        return result.Succeeded ? Result.Success() :
            Result.Failure(Error.Unauthorized(nameof(ErrorType.Unauthorized), string.Empty));
    }

    public async Task<Result> ChangePasswordAsync(string userId, string oldPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await identityContext.Users
            .SingleOrDefaultAsync(e => e.Id == userId, cancellationToken)
            .ConfigureAwait(false);

        if (user is null)
            return Result.Failure(Error.Failure("User.Password", ErrorMessages.USER_NOT_FOUND));

        var identityResult = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        return identityResult.ToApplicationResult();
    }

    public async Task<Result<string>> CreateUserAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = command.UserName,
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            IsActive = command.IsActive,
            PhoneNumber = command.PhoneNumber,
            PhotoUrl = command.PhotoUrl,
        };

        var result = await userManager.CreateAsync(user, command.Password);

        if(result.Succeeded && command.Roles?.Count > 0)
        {
            identityContext.UserRoles.AddRange(command.Roles.Select(x => new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = x
            }));

            await identityContext.SaveChangesAsync(cancellationToken);
        }

        return result.ToApplicationResult<string>(user.Id);
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), "User not found"));

        var result = await userManager.DeleteAsync(user);

        return result.Succeeded ? Result.Success() :
            Result.Failure(Error.Failure("User.Delete", "Delete user failed"));
    }

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users.FirstAsync(u => u.Id == userId, cancellationToken);

        return user.UserName;
    }

    public async Task<Result> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null) return Result.Failure(Error.NotFound(nameof(user), "User not found"));

        return await userManager.IsInRoleAsync(user, role)
            ? Result.Success()
            : Result.Failure(Error.Forbidden(nameof(ErrorType.Forbidden), "You have no permission to access the resource"));
    }
}
