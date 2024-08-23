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
    public string? Email {get; set; }
    [Required]
    public string? Password {get; set; }
    [Required]
    public string? Category {get; set; }
    [Required]
    public string? Description{get; set; }

}
