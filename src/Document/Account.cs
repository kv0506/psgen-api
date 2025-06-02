namespace PsGenApi.Document;

public class Account
{
	public string Id { get; set; } = string.Empty;

	public string UserId { get; set; } = string.Empty;

	public string Category { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	public string Username { get; set; } = string.Empty;

	public string Pattern { get; set; } = string.Empty;

	public int Length { get; set; }

	public bool IncludeSpecialCharacter { get; set; }

	public bool UseCustomSpecialCharacter { get; set; }

	public string CustomSpecialCharacter { get; set; } = string.Empty;

	public string Notes { get; set; } = string.Empty;

	public bool IsFavorite { get; set; }
}