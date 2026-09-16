using Domain.Common;

namespace Domain.Entities.SuperAdmin;
public class MenuSectionItem : AuditableEntityBase
{
    public Guid MenuSectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Href { get; set; }
    public string? Target { get; set; }
    public string? Roles { get; set; }
    public bool IsParent { get; set; }
    public int SerialNo { get; set; }
    // public virtual MenuSection MenuSection { get; set; }
    public virtual List<MenuSectionSubItem> MenuSectionSubItems { get; set; }
}
