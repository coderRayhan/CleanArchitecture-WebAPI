namespace Application.Features.MenuSections.Queries;

public class MenuSectionResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SerialNo { get; set; }
    public string Href { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool HasSubRoute { get; set; }
}