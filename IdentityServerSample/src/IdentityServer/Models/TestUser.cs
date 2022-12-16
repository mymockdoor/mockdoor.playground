namespace IdentityServer.Models;

/// <summary>
/// In-memory user object for testing. Not intended for modeling users in production.
/// </summary>
public class TestUser
{
    /// <summary>
    /// Gets or sets the subject identifier.
    /// </summary>
    public string SubjectId { get; set; }

    /// <summary>
    /// Gets or sets the username.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the provider name.
    /// </summary>
    public string ProviderName { get; set; } = "Default";

    /// <summary>
    /// Gets or sets the provider subject identifier.
    /// </summary>
    public string ProviderSubjectId { get; set; }

    /// <summary>
    /// Gets or sets if the user is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the claims.
    /// </summary>
    public ICollection<Claim> Claims { get; set; } = new List<Claim>();
}