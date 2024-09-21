using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class ResetPassword
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string? Password {get; set;}
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string? ConfirmPassword {get; set;}
    public AccountDetails? AccountDetails {get; set; }
}
