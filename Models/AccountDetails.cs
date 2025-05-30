using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApp.Security;

namespace WebApp.Models;
public class AccountDetails
{
    public AccountDetails()
    {
        initVector = Convert.ToBase64String(CryptoService.GenerateIV());
    }

    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Full Name is required")]
    public string FullName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; } = string.Empty;
    [NotMapped]
    public string ConfirmPassword { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public int AccountId { get; set; } 
    public string Role { get; set; } = "Admin";
    public DateTime DateOfBirth { get; set; } = DateTime.Now;
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool TwoFactorEnabled { get; set; }
    public bool dev_mode { get; set; } = false;
    public List<PasswordDetails>? Passwords { get; set; }
    public int AccountScore { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? TokenExpiry { get; set; }
    public ICollection<ResetPassword>? ResetPasswords { get; set; }
    public bool ActivateRememberMe {get; set;}
    [Required(ErrorMessage = "Initialization Vector is required")]
    public string initVector { get; set; } = string.Empty;
    public string? PostalCode {get; set;}
    public string? City {get; set;}
    public int AccountPoints {get; set;} // Used for Account Completion
}
