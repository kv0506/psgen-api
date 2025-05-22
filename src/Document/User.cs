namespace PsGenApi.Document;

public class User
{
    public string Id { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Mobile { get; set; } = string.Empty;

    public string Salt { get; set; } = string.Empty;

    public string Hash { get; set; } = string.Empty;
}
