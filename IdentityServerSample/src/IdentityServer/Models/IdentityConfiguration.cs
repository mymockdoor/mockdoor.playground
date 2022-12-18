namespace IdentityServer.Models;

public class IdentityConfiguration
{
    public string UserStoreUrl { get; set; }
    
    public string StoreDemoUrl { get; set; }

    public GoogleProviderConfiguration Google { get; set; }
}