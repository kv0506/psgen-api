using System.ComponentModel.DataAnnotations;

namespace PsGenApi.DataModel;

public class DbUser
{
	[Key] public Guid Id { get; set; }

	[Required] [MaxLength(100)] public string Username { get; set; } = string.Empty;

	[Required] [MaxLength(255)] public string Email { get; set; } = string.Empty;

	[MaxLength(20)] public string Mobile { get; set; } = string.Empty;

	[Required] [MaxLength(50)] public string Salt { get; set; } = string.Empty;

	[Required] [MaxLength(255)] public string Hash { get; set; } = string.Empty;

	// Navigation properties
	public virtual ICollection<DbAccount> Accounts { get; set; } = new List<DbAccount>();
	public virtual ICollection<DbToken> Tokens { get; set; } = new List<DbToken>();
}