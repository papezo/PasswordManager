using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class LoginDetails
{
    public string? Id { get; set; }
    [Required]
    public string Username{ get; set; } = string.Empty;
    [Required]
    public string Password{ get; set; } = string.Empty;
    public bool rememberMe { get; set; } = false;
}
