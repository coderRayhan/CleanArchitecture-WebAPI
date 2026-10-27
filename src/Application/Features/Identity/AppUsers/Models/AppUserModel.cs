namespace Application.Features.Identity.AppUsers.Models;

public class AppUserModel
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AssignedRoles { get; set; } = string.Empty;

    public IList<string>? Roles { get; set; } = [];
}