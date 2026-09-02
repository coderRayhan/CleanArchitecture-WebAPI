namespace Application.Common.DapperQueries;

public interface IDataGrid : IPaginated, ISortable, IGlobalFilterable
{
    bool? AllowCache { get; set; }
}

public interface IPaginated
{
    int PageNumber { get; }
    int PageSize { get; set; }
    int Offset { get; set; }
}

public interface ISortable
{
    string SortField { get; set; }
    int? SortOrder { get; set; }
    string? DefaultSortField { get; set; }
}

public interface IGlobalFilterable
{
    string GlobalFilterValue { get; set; }
}