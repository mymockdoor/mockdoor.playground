namespace IdentityServer.Models;

public class Claim
{
    public string Type { get; set; }
    
    public string Value { get; set; }

    public Claim()
    {
        
    }

    public Claim(string type, string value)
    {
        Type = type;
        Value = value;  
    }
}