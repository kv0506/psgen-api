namespace PsGenApi.ServiceModel;

public class CreateAccountDto : AccountBaseDto
{
}

public class UpdateAccountDto : AccountBaseDto
{
	public string Id { get; set; } = string.Empty;
}

public class DeleteAccountDto
{
	public string Id { get; set; } = string.Empty;
}

public class AccountBaseDto
{
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

public class AccountDto : AccountBaseDto
{
	public string Id { get; set; } = string.Empty;
}