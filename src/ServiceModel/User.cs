namespace PsGenApi.ServiceModel;

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class CreateUser
{
    public string Username { get; set; }
    
    public string Password { get; set; }

    public string Email { get; set; }

    public string Mobile { get; set; }
}

public class UpdateUser
{
    public string Id { get; set; }

    public string CurrentPassword { get; set; }

    public string NewPassword { get; set; }

    public string Email { get; set; }

    public string Mobile { get; set; }
}

public class User
{
    public string Id { get; set; }

    public string Username { get; set; }

    public string Email { get; set; }

    public string Mobile { get; set; }
}

public class UserSession
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string Secret { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }
}