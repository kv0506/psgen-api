namespace PsGenApi.ServiceModel;

public class LoginRequestDto
{
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}

public class CreateUserDto
{
	public string Username { get; set; } = string.Empty;

	public string Password { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public string Mobile { get; set; } = string.Empty;
}

public class UpdateUserDto
{
	public string Id { get; set; } = string.Empty;

	public string CurrentPassword { get; set; } = string.Empty;

	public string NewPassword { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public string Mobile { get; set; } = string.Empty;
}

public class UserDto
{
	public string Id { get; set; } = string.Empty;

	public string Username { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;

	public string Mobile { get; set; } = string.Empty;
}

public class UserSessionDto
{
	public string Id { get; set; } = string.Empty;

	public string UserId { get; set; } = string.Empty;

	public string Secret { get; set; } = string.Empty;

	public DateTimeOffset ExpiresAt { get; set; }
}