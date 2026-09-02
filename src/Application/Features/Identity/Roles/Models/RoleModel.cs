namespace Application.Features.Identity.Roles.Models;

public class RoleModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = [];
}