using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Models;

public class PasswordDetails
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string? Password { get; set; }
    [Required]
    public string? Category { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime PasswordAge { get; set; } = DateTime.Now;
    public bool IsPasswordOld { get; set; }
    public bool PasswordHas2FA { get; set; }
    public string? OneTimePassword { get; set; }
    [ForeignKey("AccountDetails")]
    public int AccountDetailsId { get; set; }
    public AccountDetails? AccountDetails { get; set; }
    public int PasswordScore { get; set; }
    public bool PasswordSecure {get; set;}
    public bool DuplicatedPassword {get; set;}
    public byte[]? ImagePath {get; set;}
    public string? BackUpCodes {get; set;}
    public bool IsFavorite {get; set;}
    public string initVector{get; set;}
}
