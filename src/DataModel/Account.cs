using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PsGenApi.DataModel;

public class DbAccount
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(128)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(128)]
    public string Pattern { get; set; } = string.Empty;

    public int Length { get; set; }

    public bool IncludeSpecialCharacter { get; set; }

    public bool UseCustomSpecialCharacter { get; set; }

    [MaxLength(2)]
    public string CustomSpecialCharacter { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public bool IsFavorite { get; set; }

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public virtual DbUser User { get; set; } = null!;
}
