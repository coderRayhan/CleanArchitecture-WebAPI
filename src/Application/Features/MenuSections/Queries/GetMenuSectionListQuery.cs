using System.Text.Json.Serialization;
using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Application.Common.DapperQueries;
using Domain.Shared;

namespace Application.Features.MenuSections.Queries;

public record GetMenuSectionListQuery : DataGridModel, ICacheableQuery<PaginatedResponse<MenuSectionResponse>>
{
    [JsonIgnore]
    public string CacheKey => $"MenuSections_{PageNumber}_{PageSize}";
}

internal class GetMenuSectionListQueryHandler(
    ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetMenuSectionListQuery, PaginatedResponse<MenuSectionResponse>>
{
    public async Task<Result<PaginatedResponse<MenuSectionResponse>>> Handle(GetMenuSectionListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();
        var sql = """
                    SELECT *
                    FROM MenuSections AS MS
                    WHERE 1 = 1
                   """;

        return await PaginatedResponse<MenuSectionResponse>
            .CreateAsync(
            connection,
            sql,
            request);
        
    }
}