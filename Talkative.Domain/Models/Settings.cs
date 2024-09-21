namespace Talkative.Domain.Models;

public class Settings
{
    public string BearerKey { get; set; } = null!;
    public int Expiration { get; set; }
    public string Issuer { get; set; } = null!;
}