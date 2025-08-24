using Application.Common.Models;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.Json.Serialization;

namespace Application.Common.Abstractions.Dapper
{
    public class DapperPaginatedResponse<TEntity>
        where TEntity : class
    {
        [JsonInclude]
        public IReadOnlyCollection<TEntity> Items { get; init; }
        public int PageNumber { get; init; }
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public DapperPaginatedResponse()
        {
            
        }

        public DapperPaginatedResponse(
            IReadOnlyCollection<TEntity> items,
            int count,
            int pageNumber,
            int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }

        public static async Task<DapperPaginatedResponse<TEntity>> CreateAsync(
            IDbConnection dbConnection,
            string sql,
            DataGridModel gridModel,
            object? parameters = default)
        {
            var logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger<DapperPaginatedResponse<TEntity>>>();

            var paginatedSql = $"""
                {sql}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
                """;

            #region Parameters
            var param = new DynamicParameters(parameters);
            param.Add(nameof(gridModel.Offset), gridModel.Offset);
            param.Add(nameof(gridModel.PageSize), gridModel.PageSize);
            #endregion

            logger?.LogInformation("Executing SQL: {Sql}", paginatedSql);

            var items = await dbConnection
                .QueryAsync<TEntity>(paginatedSql, param);

            var count = items.Count();

            return new DapperPaginatedResponse<TEntity>(
                items.AsList(),
                count,
                gridModel.PageNumber,
                gridModel.PageSize);
        }

    }
}
