using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class PasswordDetails
{
    [Key]
    public int Id {get; set; }
    [Required]
    public string? Name {get; set; }
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email {get; set; }
    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string? Password {get; set; }
    [Required]
    public string? Category {get; set; }
    [Required]
    public string? Description{get; set; }
    public DateTime CreatedAt {get; set; } = DateTime.Now;

}
