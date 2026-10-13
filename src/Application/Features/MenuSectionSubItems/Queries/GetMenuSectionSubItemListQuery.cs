using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Application.Common.DapperQueries;
using Domain.Shared;

namespace Application.Features.MenuSectionSubItems.Queries;

public sealed record GetMenuSectionSubItemListQuery : DataGridModel, ICacheableQuery<PaginatedResponse<MenuSectionSubItemResponse>>
{
    public string CacheKey => $"MenuSectionSubItems_{PageNumber}_{PageSize}";
}

internal sealed class GetMenuSectionSubItemListQueryHandler(
    ISqlConnectionFactory sqlConnection)
    : IQueryHandler<GetMenuSectionSubItemListQuery, PaginatedResponse<MenuSectionSubItemResponse>>
{
    public async Task<Result<PaginatedResponse<MenuSectionSubItemResponse>>> Handle(GetMenuSectionSubItemListQuery request, CancellationToken cancellationToken)
    {
        var connection = sqlConnection.GetOpenConnection();
        var sql = $"""
                   SELECT
                   	    MSSI.Id,
                   	    MSSI.MenuSectionItemId,
                   	    MSI.Title ${nameof(MenuSectionSubItemResponse.MenuSectionItemTitle)},
                   	    MSSI.Title,
                   	    MSSI.Href,
                   	    MSSI.SerialNo
                   FROM MenuSectionSubItems MSSI
                   INNER JOIN MenuSectionItems MSI ON MSSI.MenuSectionItemId = MSI.Id
                   """;
        return await PaginatedResponse<MenuSectionSubItemResponse>
            .CreateAsync(
            connection,
            sql,
            request);
    }
}
