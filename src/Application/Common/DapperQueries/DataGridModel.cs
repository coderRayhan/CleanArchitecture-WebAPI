using System.Text.Json.Serialization;

namespace Application.Common.DapperQueries;

public abstract record DataGridModel : IDataGrid
{
    public bool IsInitialLoaded { get; set; } = false;
    
    // Pagination
    public int PageNumber { get; } = 1;
    public int PageSize { get; set; } = 10;
    public int Offset { get; set; } = 0;
    
    // Sortable
    public string SortField { get; set; }
    public int? SortOrder { get; set; }
    public string? DefaultSortField { get; set; }
    
    // Caching
    public virtual bool? AllowCache { get; set; }
    [JsonIgnore] public TimeSpan? Expiration { get; set; } = null;
    public string GlobalFilterValue { get; set; } = string.Empty;
}