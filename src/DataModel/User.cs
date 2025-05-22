using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PsGenApi.DataModel;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; }

    [MaxLength(20)]
    public string Mobile { get; set; }

    [Required]
    [MaxLength(50)]
    public string Salt { get; set; }

    [Required]
    [MaxLength(255)]
    public string Hash { get; set; }

    // Navigation properties
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
}
