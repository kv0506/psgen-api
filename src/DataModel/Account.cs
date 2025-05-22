using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PsGenApi.DataModel;

public class Account
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(128)]
    public string Category { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; }

    [MaxLength(255)]
    public string Username { get; set; }

    [MaxLength(128)]
    public string Pattern { get; set; }

    public int Length { get; set; }

    public bool IncludeSpecialCharacter { get; set; }

    public bool UseCustomSpecialCharacter { get; set; }

    [MaxLength(2)]
    public string CustomSpecialCharacter { get; set; }

    public string Notes { get; set; }

    public bool IsFavorite { get; set; }

    // Navigation property
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }
}
