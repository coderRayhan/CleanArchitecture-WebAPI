using System.Text.Json.Serialization;
using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Application.Common.DapperQueries;
using Application.Common.Models;
using Application.Features.Identity.Roles.Models;
using Domain.Shared;

namespace Application.Features.Identity.Roles.Queries;

public sealed record GetRoleListQuery : DataGridModel, ICacheableQuery<PaginatedResponse<RoleModel>>
{
    public string CacheKey => $"Roles:{Offset}:{PageSize}";
}

internal sealed class GetRoleListQueryHandler(ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetRoleListQuery, PaginatedResponse<RoleModel>>
{
    public async Task<Result<PaginatedResponse<RoleModel>>> Handle(GetRoleListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
                   SELECT 
                       R.Id AS {nameof(RoleModel.Id)}, 
                       R.Name AS {nameof(RoleModel.Name)} 
                   FROM [identity].Roles AS R
                   """;

        return await PaginatedResponse<RoleModel>
            .CreateAsync(connection, sql, request);

    }
}