using Application.Common.Abstractions.Contracts;
using Application.Common.Abstractions.Identity;
using Application.Features.Identity.Roles.Models;
using Domain.Shared;

namespace Application.Features.Identity.Roles.Queries;

public sealed record GetRolePermissionsQuery(string roleId) : IQuery<RoleModel>;

internal sealed class GetRolePermissionsQueryHandler(
    IIdentityRoleService roleService)
: IQueryHandler<GetRolePermissionsQuery, RoleModel>
{
    public async Task<Result<RoleModel>> Handle(GetRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        return await roleService.GetRolePermissionsAsync(request.roleId, cancellationToken);
    }
}