
namespace ClientApiService.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public static class UserRoles
{
    public const string Admin = "Admin";
    public const string Operator = "Operator";
    public const string Viewer = "Viewer";
}