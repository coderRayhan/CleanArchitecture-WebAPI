namespace Application.Features.MenuSectionItems.Queries;

public class MenuSectionItemResponse
{
    public Guid MenuSectionId { get; set; }
    public string MenuSectionTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Href { get; set; }
    public string? Target { get; set; }
    public string? Roles { get; set; }
    public bool IsParent { get; set; }
    public int SerialNo { get; set; }
}
