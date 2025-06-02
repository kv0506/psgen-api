namespace PsGenApi.Document;

public class Token
{
	public string Id { get; set; } = string.Empty;

	public string Secret { get; set; } = string.Empty;

	public string UserId { get; set; } = string.Empty;

	public DateTimeOffset ExpiresAt { get; set; }
}