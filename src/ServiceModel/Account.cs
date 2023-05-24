namespace PsGenApi.ServiceModel;

public class CreateAccount : AccountBase
{
}

public class UpdateAccount : AccountBase
{
    public string Id { get; set; }
}

public class DeleteAccount
{
    public string Id { get; set; }
}

public class AccountBase
{
    public string Category { get; set; }

    public string Name { get; set; }

    public string Pattern { get; set; }

    public int Length { get; set; }

    public bool IncludeSpecialCharacter { get; set; }

    public bool UseCustomSpecialCharacter { get; set; }

    public string CustomSpecialCharacter { get; set; }
}


public class Account : AccountBase
{
    public string Id { get; set; }
}