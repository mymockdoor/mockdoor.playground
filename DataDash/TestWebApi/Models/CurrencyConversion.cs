namespace TestWebApi.Models;

public class CurrencyConversion
{
    public Motd motd { get; set; }
    public bool success { get; set; }
    public string @base { get; set; }
    public DateTime date { get; set; }
    public Dictionary<string, double> rates { get; set; }
}

public class Motd
{
    public string msg { get; set; }
    public string url { get; set; }
}
