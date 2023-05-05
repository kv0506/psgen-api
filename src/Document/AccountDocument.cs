namespace PsGenApi.Document;

public class AccountDocument
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string Category { get; set; }

    public string Name { get; set; }

    public string Pattern { get; set; }

    public string Length { get; set; }

    public bool IncludeSpecialCharacter { get; set; }

    public bool UseCustomSpecialCharacter { get; set; }

    public bool CustomSpecialCharacter { get; set; }
}