namespace UserStore.Models;

public class ProvisionUserDto
{
    /// <summary>
    /// Gets or sets the provider name.
    /// </summary>
    public string ProviderName { get; set; } = "Default";

    /// <summary>
    /// Gets or sets the provider subject identifier.
    /// </summary>
    public string ProviderSubjectId { get; set; }

    /// <summary>
    /// Gets or sets the claims.
    /// </summary>
    public ICollection<Claim> Claims { get; set; } = new List<Claim>();
}