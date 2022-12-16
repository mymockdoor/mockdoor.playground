using Microsoft.AspNetCore.Mvc;
using UserPermissions.Models;

namespace UserPermissions.Controllers;

[ApiController]
[Route("[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(ILogger<PermissionsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{id}")]
    public List<string> GetPermission(int id)
    {
        return PermissionsList.Permissions.FirstOrDefault(p => p.UserId == id)?.Permissions;
    }
}