namespace PsGenApi.Document;

public class TokenDocument
{
    public string Id { get; set; }

    public string Secret { get; set; }

    public string UserId { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
}