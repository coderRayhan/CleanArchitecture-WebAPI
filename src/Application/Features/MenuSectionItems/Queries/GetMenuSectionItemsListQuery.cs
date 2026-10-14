using Application.Common.Abstractions;
using Application.Common.Abstractions.Contracts;
using Application.Common.DapperQueries;
using Domain.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MenuSectionItems.Queries
{
    public sealed record GetMenuSectionItemsListQuery : DataGridModel, ICacheableQuery<PaginatedResponse<MenuSectionItemResponse>>
    {
        public string CacheKey => $"MenuSectionItems_{PageNumber}_{PageSize}";
    }

    internal sealed class GetMenuSectionItemsListQueryHandler(
        ISqlConnectionFactory sqlConnection)
        : IQueryHandler<GetMenuSectionItemsListQuery, PaginatedResponse<MenuSectionItemResponse>>
    {
        public async Task<Result<PaginatedResponse<MenuSectionItemResponse>>> Handle(GetMenuSectionItemsListQuery request, CancellationToken cancellationToken)
        {
            var connection = sqlConnection.GetOpenConnection();
            var sql = $"""
                    SELECT
                        MSI.Id,
                   	    MSI.MenuSectionId,
                    	MS.Title {nameof(MenuSectionItemResponse.MenuSectionTitle)},
                   	    MSI.Title,
                   	    MSI.Href,
                   	    MSI.IsParent,
                   	    MSI.SerialNo,
                   	    MSI.IsDeleted,
                   	    MSI.IsActive
                   FROM MenuSectionItems MSI
                   INNER JOIN MenuSections MS ON MSI.MenuSectionId = MS.Id
                   """;
            return await PaginatedResponse<MenuSectionItemResponse>
                .CreateAsync(
                connection,
                sql,
                request);
        }
    }
}
