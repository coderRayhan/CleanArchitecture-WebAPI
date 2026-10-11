using Application.Common.Models;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;
using Application.Common.Abstractions;

namespace Application.Common.DapperQueries
{
    public class PaginatedResponse<TEntity>
        where TEntity : class
    {
        [JsonInclude]
        public IReadOnlyCollection<TEntity> Items { get; init; }
        public int PageNumber { get; init; }
        public int TotalPages { get; init; }
        public int TotalCount { get; init; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PaginatedResponse()
        {
            
        }

        public PaginatedResponse(
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

        public static async Task<PaginatedResponse<TEntity>> CreateAsync(
            IDbConnection dbConnection,
            string sql,
            DataGridModel gridModel,
            IEnumerable<GlobalFilterColumn> filters = null,
            object? parameters = default)
        {
            var logger = ServiceLocator.ServiceProvider.GetRequiredService<ILogger<PaginatedResponse<TEntity>>>();

            if (filters is not null)
            {
                SetWhereClauseIfNotExist(ref sql);
                SetGlobalFilters(gridModel, filters, ref sql);
            }
            
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
            
            var count = await dbConnection
                .ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM ({sql}) as CountQuery", param);

            return new PaginatedResponse<TEntity>(
                items.AsList(),
                count,
                gridModel.PageNumber,
                gridModel.PageSize);
        }

        private static int GetOffset(int pageSize, int pageNumber)
        {
            return (pageNumber - 1) * pageSize;
        }

        #region Filters

        private static bool HasWhereClause(string sql)
        {
            return sql.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static void SetWhereClauseIfNotExist(ref string sql)
        {
            if (HasWhereClause(sql))
            {
                return;
            }
            string defaultWhereSql = $"WHERE 1 = 1";

            sql = $"""
                   {sql} 
                   {defaultWhereSql} 
                   """;
        }

        private static void SetGlobalFilters(DataGridModel gridModel, IEnumerable<GlobalFilterColumn> filters, ref string sql)
        {
            if (string.IsNullOrEmpty(gridModel.GlobalFilterValue) ||
                string.IsNullOrWhiteSpace(gridModel.GlobalFilterValue))
            {
                return;
            }

            StringBuilder globalFilter = new();
            
            bool isFirst = true;
            foreach (var field in filters)
            {
                if (isFirst)
                {
                    globalFilter.Append('(');
                    isFirst = false;
                }
                else
                {
                    globalFilter.Append(" OR ");
                }

                if (field.Type == FilterColumnType.Date)
                {
                    globalFilter.Append($"CONVERT(NVARCHAR(10), {field.Name}, 103) LIKE @GlobalFilterValue");
                }
                else
                {
                    globalFilter.Append($"LOWER({field.Name}) LIKE @GlobalFilterValue");
                }
            }

            if (!isFirst) globalFilter.Append(')');

            sql = $"""
                   {sql} 
                   AND {globalFilter}
                   """;
        }

        #endregion

        #region Sorting Functions (ORDER BY)

        private static string GetOrderBySql(DataGridModel gridModel)
        {
            if (string.IsNullOrEmpty(gridModel.SortField) || gridModel.SortField is null)
            {
                return string.Empty;
            }

            return gridModel.SortOrder == -1
                ? $"ORDER BY {gridModel.SortField} DESC"
                : $"ORDER BY {gridModel.SortField} ASC";
        }

        private static bool HasOrderByClause(string sql)
        {
            return sql.Contains($"ORDER BY", StringComparison.OrdinalIgnoreCase);
        }


        #endregion
        
        public enum FilterColumnType { Text, Date, DateTime }

        public sealed record GlobalFilterColumn(string Name, FilterColumnType Type = FilterColumnType.Text)
        {
            // lets you keep writing plain strings for text columns
            public static implicit operator GlobalFilterColumn(string name) => new(name);
        }
    }
}
