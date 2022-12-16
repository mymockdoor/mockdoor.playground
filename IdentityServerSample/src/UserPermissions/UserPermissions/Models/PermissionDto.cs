namespace UserPermissions.Models;

public class PermissionDto
{
    public int UserId { get; set; }

    public List<string> Permissions { get; set; }

    public PermissionDto()
    {
        
    }

    public PermissionDto(int userId, params string[] permissions)
    {
        UserId = userId;
        Permissions = permissions.ToList();
    }
}