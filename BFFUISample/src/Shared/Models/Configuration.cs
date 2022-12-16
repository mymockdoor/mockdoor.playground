namespace Blazor6.Shared.Models;

public class Configuration
{
    public ServiceUrls ServiceUrls { get; set; }

    public string ClientId { get; set; }

    public List<string> Scopes { get; set; }
}