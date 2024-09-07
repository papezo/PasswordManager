using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class AccountDetails
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;
        public int CustomerId {get; set;}
        public int AccountId { get; set; } 
        public string Role {get; set;} = "user";
        public DateTime DateOfBirth {get; set; } = DateTime.Now;
        public string? Address {get; set;}
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool TwoFactorEnabled {get; set;}
        public bool dev_mode {get; set; } = false;
    }
}