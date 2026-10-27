using Application.Common.Abstractions.Identity;
using Application.Common.Constants;
using Domain.Shared;
using Infrastructure.Identity.Permissions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Features.Identity.Roles.Models;

namespace Infrastructure.Identity.Services;
internal class IdentityRoleService(
    RoleManager<IdentityRole> roleManager,
    IdentityContext context,
    ILogger<IdentityRoleService> logger) 
    : IIdentityRoleService
{
    public async Task<Result> AddorRemoveClaimsToRoleAsync(string id, List<string> permissions, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role is null) return Result.Failure(Error.Failure("Role.Update", ErrorMessages.ROLE_NOT_FOUND));

        var existedPermissions = await context.RoleClaims.Where(e => e.RoleId == role.Id).ToListAsync(cancellationToken);

        context.RoleClaims.RemoveRange(existedPermissions);

        var newPermissions = permissions.Select(x => new IdentityRoleClaim<string>
        {
            RoleId = role.Id,
            ClaimType = CustomClaimTypes.Permission,
            ClaimValue = x
        });

        context.RoleClaims.AddRange(newPermissions);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<RoleModel>> GetRolePermissionsAsync(string id, CancellationToken cancellationToken = default)
    {
        var role = await roleManager.FindByIdAsync(id);
        if(role is null)
            Result.Failure<RoleModel>(Error.Failure("Get role permissions failed", ErrorMessages.ROLE_NOT_FOUND));
        
        var permissions = await roleManager.GetClaimsAsync(role!);

        return Result.Success(new RoleModel()
        {
            Id = role!.Id,
            Name = role.Name,
            Permissions = permissions.Select(x => x.Value).ToList()
        });
    }

    public async Task<Result<string>> CreateRoleAsync(string name, CancellationToken cancellationToken)
    {
        try
        {
            var role = new IdentityRole
            {
                Name = name,
                NormalizedName = name.ToUpper()
            };

            var result = await roleManager.CreateAsync(role);

            return result.Succeeded ? Result.Success(role.Id)
                : Result.Failure<string>(Error.Failure("Role.Create", MapErrors(result.Errors)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occured to save role");
            return Result.Failure<string>(Error.Failure("Role.Create", "Error occured to save role"));
        }
    }

    public async Task<Result> DeleteRoleAsync(string id, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role is null) return Result.Failure(Error.Failure("Role.Delete", ErrorMessages.ROLE_NOT_FOUND));
        
        var result = await roleManager.DeleteAsync(role);

        return result.Succeeded ?
            Result.Success()
            : Result.Failure(Error.Failure("Role.Delete", ErrorMessages.UNABLE_DELETE_ROLE));
    }

    public async Task<Result<string>> GetRoleAsync(string id, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role is null) return Result.Failure<string>(Error.Failure("Role.Delete", ErrorMessages.ROLE_NOT_FOUND));

        return Result.Success(await roleManager.GetRoleNameAsync(role));
    }

    public async Task<Result> UpdateRoleAsync(string id, string name, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(id);

        if (role is null) return Result.Failure(Error.Failure("Role.Update", ErrorMessages.ROLE_NOT_FOUND));

        role.Name = name;
        role.NormalizedName = name.ToUpper();

        var result = await roleManager.UpdateAsync(role);

        return result.Succeeded ?
            Result.Success() :
            Result.Failure(Error.Failure("Role.Update", MapErrors(result.Errors)));
    }

    private string MapErrors(IEnumerable<IdentityError> errors)
    {
        var errorList = errors.Select(e => $"{e.Code} : {e.Description}");
        return string.Join("; ", errorList);
    }
}
