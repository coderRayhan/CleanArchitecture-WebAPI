
using Domain.Common;

namespace Domain.Entities.SuperAdmin;
public class MenuSectionSubItem : AuditableEntityBase
{
    public Guid MenuSectionItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Href { get; set; }
    public string? Roles { get; set; }
    public string? Target { get; set; }
    public int SerialNo { get; set; }
    // public virtual MenuSectionItem MenuSectionItem { get; set; }
}
