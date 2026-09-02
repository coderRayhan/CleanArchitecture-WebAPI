using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Domain.Shared;
using System.Text.Json.Serialization;
using Application.Common.Security;
using Application.Common.DapperQueries;

namespace Application.Features.Lookups.Queries;
[Authorize(Policy = Permissions.CommonSetup.Lookups.View)]
public record GetLookupListQuery : DataGridModel, ICacheableQuery<PaginatedResponse<LookupResponse>>
{
    [JsonIgnore]
    public string CacheKey => $"Lookups_{PageNumber}_{PageSize}";
}

internal sealed class GetLookupListQueryHandler(
    ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetLookupListQuery, PaginatedResponse<LookupResponse>>
{
    public async Task<Result<PaginatedResponse<LookupResponse>>> Handle(GetLookupListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();

        var sql = $"""
            SELECT *
            FROM dbo.Lookups AS l
            LEFT JOIN dbo.Lookups AS parent ON l.ParentId = parent.Id
            WHERE 1 = 1
            --AND CONCAT(l.Name, parent.Name) LIKE '%{request.GlobalFilterValue}%'
            """;

        return await PaginatedResponse<LookupResponse>
            .CreateAsync(connection, sql, request);
    }
}
