using Domain.Common;

namespace Domain.Entities.SuperAdmin;
public class MenuSection : AuditableEntityBase
{
    public string Title { get; set; } = string.Empty;
    public int SerialNo { get; set; }
    public string Href { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool HasSubRoute { get; set; }
    public virtual List<MenuSectionItem> MenuSectionItems { get; set; }
}
