namespace Application.Features.MenuSectionSubItems.Queries;

public sealed class MenuSectionSubItemResponse
{
    public Guid Id { get; set; }
    public Guid MenuSectionItemId { get; set; }
    public string MenuSectionItemTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Href { get; set; }
    public string? Roles { get; set; }
    public string? Target { get; set; }
    public int SerialNo { get; set; }
    public bool IsActive { get; set; }
}
