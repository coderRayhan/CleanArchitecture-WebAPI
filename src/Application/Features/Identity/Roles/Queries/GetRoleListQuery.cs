using System.Text.Json.Serialization;
using Application.Common.Abstractions.Contracts;
using Application.Common.DapperQueries;
using Application.Common.Models;
using Application.Features.Identity.Roles.Models;

namespace Application.Features.Identity.Roles.Queries;

public sealed record GetRoleListQuery : DataGridModel, ICacheableQuery<PaginatedList<RoleModel>>
{
    public string CacheKey { get; }
}