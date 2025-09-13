using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Application.Common.Models;
using Domain.Shared;
using System.Text.Json.Serialization;
using Application.Common.Security;
using Application.Common.Abstractions.Dapper;

namespace Application.Features.Lookups.Queries;
[Authorize(Policy = Permissions.CommonSetup.Lookups.View)]
public record GetLookupListQuery : DataGridModel, ICacheableQuery<DapperPaginatedResponse<LookupResponse>>
{
    [JsonIgnore]
    public string CacheKey => $"Lookups_{PageNumber}_{PageSize}";
    [JsonIgnore]
    public TimeSpan? Expiration { get; set; } = null;

    public bool? AllowCache { get; set; } = true;
}

internal sealed class GetLookupListQueryHandler(
    ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetLookupListQuery, DapperPaginatedResponse<LookupResponse>>
{
    public async Task<Result<DapperPaginatedResponse<LookupResponse>>> Handle(GetLookupListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT *
            FROM dbo.Lookups AS l
            LEFT JOIN dbo.Lookups AS parent ON l.ParentId = parent.Id
            WHERE 1 = 1
            --AND CONCAT(l.Name, parent.Name) LIKE '%{request.GlobalFilterText}%'
            """;

        return await DapperPaginatedResponse<LookupResponse>
            .CreateAsync(connection, sql, request);
    }
}
