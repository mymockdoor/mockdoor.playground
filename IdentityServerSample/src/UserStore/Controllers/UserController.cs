using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using UserStore.Models;
using Claim = UserStore.Models.Claim;

namespace UserStore.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    [Route("authenticate")]
    public ActionResult<IEnumerable<TestUser>> Authenticate([FromBody] AuthenticateDto authenticateRequest)
    {
        _logger.LogInformation("logging user in");
        if (string.IsNullOrWhiteSpace(authenticateRequest?.Username) ||
            string.IsNullOrWhiteSpace(authenticateRequest.Password) ||
            string.IsNullOrWhiteSpace(authenticateRequest.ProviderName))
        {
            return BadRequest("invalid username, password or provider");
        }

        var matchingUser = TestUsers.Users.FirstOrDefault(u => u.IsActive &&
                                                               u.Username.ToUpper() == authenticateRequest.Username.ToUpper() &&
                                                               u.Password == authenticateRequest.Password &&
                                                               u.ProviderName.ToUpper() ==
                                                               authenticateRequest.ProviderName.ToUpper());
        
        if(matchingUser != null)
            return Ok(matchingUser);

        return NotFound();
    }
    
    [HttpPost]
    [Route("provision")]
    public ActionResult<IEnumerable<TestUser>> Authenticate([FromBody] ProvisionUserDto provision)
    {
        _logger.LogInformation("logging user in");
        if (string.IsNullOrWhiteSpace(provision?.ProviderName) ||
            string.IsNullOrWhiteSpace(provision.ProviderSubjectId))
        {
            return BadRequest("invalid username, password or provider");
        }

        var matchingUser = TestUsers.Users.FirstOrDefault(u => u.IsActive &&
                                                               u.ProviderName.ToUpper() == provision.ProviderName.ToUpper() &&
                                                               u.ProviderSubjectId == provision.ProviderSubjectId);

        
        if(matchingUser != null)
            return Conflict();

        
        return Ok(AutoProvisionUser(provision.ProviderName, provision.ProviderSubjectId, provision.Claims.ToList()));
    }

    [HttpGet]
    [Route("findbyprovider/{providerName}/{providerId}")]
    public ActionResult<IEnumerable<TestUser>> Authenticate(string providerName, string providerId)
    {
        _logger.LogInformation("finding user in");
        if (string.IsNullOrWhiteSpace(providerName) ||
                                      string.IsNullOrWhiteSpace(providerId))
        {
            return BadRequest("invalid username, password or provider");
        }

        var matchingUser = TestUsers.Users.FirstOrDefault(u => u.IsActive &&
                                                               u.ProviderName.ToUpper() == providerName.ToUpper() &&
                                                               u.ProviderSubjectId == providerId);
        
        if(matchingUser != null)
            return Ok(matchingUser);

        return NotFound();
    }

    /// <summary>
    /// Automatically provisions a user.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="claims">The claims.</param>
    /// <returns></returns>
    private TestUser AutoProvisionUser(string provider, string userId, List<Claim> claims)
    {
        // create a list of claims that we want to transfer into our store
        var filtered = new List<Claim>();

        foreach (var claim in claims)
        {
            // if the external system sends a display name - translate that to the standard OIDC name claim
            if (claim.Type == ClaimTypes.Name)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
            }
            // if the JWT handler has an outbound mapping to an OIDC claim use that
            else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
            {
                filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
            }
            // copy the claim as-is
            else
            {
                filtered.Add(claim);
            }
        }

        // if no display name was provided, try to construct by first and/or last name
        if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
        {
            var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
            var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
            if (first != null && last != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
            }
            else if (first != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, first));
            }
            else if (last != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, last));
            }
        }

        // create a new unique subject id
        var sub = CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);

        // check if a display name is available, otherwise fallback to subject id
        var name = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name)?.Value ?? sub;

        // create new user
        var user = new TestUser
        {
            SubjectId = sub,
            Username = name,
            ProviderName = provider,
            ProviderSubjectId = userId,
            Claims = filtered
        };

        // add user to in-memory store
        TestUsers.Users.Add(user);

        return user;
    }
}