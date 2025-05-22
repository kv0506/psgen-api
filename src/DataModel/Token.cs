using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PsGenApi.DataModel;

public class Token
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Secret { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public DateTimeOffset ExpiresAt { get; set; }

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
}
