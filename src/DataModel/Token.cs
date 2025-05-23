using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PsGenApi.DataModel;

public class DbToken
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Secret { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public DateTimeOffset ExpiresAt { get; set; }

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public virtual DbUser User { get; set; } = null!;
}
