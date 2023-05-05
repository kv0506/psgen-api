namespace PsGenApi.Document;

public class UserDocument
{
    public string Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string Mobile { get; set; }

    public string Salt { get; set; }

    public string Hash { get; set; }
}