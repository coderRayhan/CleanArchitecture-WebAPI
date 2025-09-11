using Application.Common.Models;
using Dapper;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
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

            var offset = GetOffset(gridModel.PageSize, gridModel.PageNumber);

            string orderBy = string.Empty;
            if (!HasOrderByClause(sql))
            {
                string orderBySql = GetOrderBySql(gridModel);
                orderBy = string.IsNullOrEmpty(orderBySql) ? "ORDER BY (SELECT NULL)" : orderBySql;
            }

            var paginatedSql = $"""
                {sql}
                {orderBy}
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                """;

            #region Parameters
            var param = new DynamicParameters(parameters);
            param.Add("Offset", offset);
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

        private static int GetOffset(int pageSize, int pageNumber)
        {
            return (pageNumber - 1) * pageSize;
        }

        #region Sorting Functions (ORDER BY)

        private static string GetOrderBySql(DataGridModel gridModel)
        {
            if (string.IsNullOrEmpty(gridModel.SortField) || gridModel.SortField is null)
            {
                return string.Empty;
            }

            return gridModel.SortingDirection == -1
                ? $"ORDER BY {gridModel.SortField} DESC"
                : $"ORDER BY {gridModel.SortField} ASC";
        }

        private static bool HasOrderByClause(string sql)
        {
            return sql.Contains($"ORDER BY", StringComparison.OrdinalIgnoreCase);
        }


        #endregion
    }
}
