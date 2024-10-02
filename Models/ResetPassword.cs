
using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApp.Models;

public class ResetPassword
{
    public int Id { get; set; }
    [ValidateNever]
    public string? Token {get; set;}
    [ValidateNever]
    public string? Email {get; set;}
    public DateTime TokenExpiry {get; set;}
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string? Password {get; set;}
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string? ConfirmPassword {get; set;}
    public int AccountDetailsId { get; set; }
    public AccountDetails? AccountDetails {get; set; }
}