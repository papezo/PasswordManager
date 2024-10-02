using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Dto;

public class ForgotPasswordDto
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
    [Required]
    public string? ClientUri { get; set; }
}
