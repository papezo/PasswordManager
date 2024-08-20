using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class LoginDetails
{
    [Required]
    public string Username{ get; set; } = string.Empty;
    [Required]
    public string Password{ get; set; } = string.Empty;
}
